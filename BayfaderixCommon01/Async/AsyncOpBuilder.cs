namespace Name.Bayfaderix.Darxxemiyur.Common.Async;

public sealed class AsyncOpBuilder
{
	public static TaskScheduler GetScheduler(TaskScheduler? suggested = default)
	{
		if (suggested != null)
			return suggested;

		if (SynchronizationContext.Current is IMyUnderlyingContext context && context.MyTaskScheduler != null)
			return context.MyTaskScheduler;

		//TaskScheduler.FromCurrentSynchronizationContext() with current as null would probably return TaskScheduler.Default, but I'm not riskin it.
		if (SynchronizationContext.Current is not null)
			return TaskScheduler.FromCurrentSynchronizationContext();

		return TaskScheduler.Current;
	}

	public static AsyncOpBuilder Get => new AsyncOpBuilder();
	private readonly LinkedList<Func<Task>> _unCancellableTasks;
	private readonly LinkedList<Func<CancellationToken, Task>> _cancellableTasks;
	private readonly LinkedList<IAsyncRunnable> _asyncRunnables;
	private CancellationToken? _token;
	private TaskCreationOptions? _options;
	private TaskFactory? _factory;
	private TaskScheduler? _scheduler;

	public AsyncOpBuilder()
	{
		_unCancellableTasks = new();
		_cancellableTasks = new();
		_asyncRunnables = new();
	}

	public AsyncOpBuilder(AsyncOpBuilder oop)
	{
		_unCancellableTasks = oop._unCancellableTasks;
		_cancellableTasks = oop._cancellableTasks;
		_asyncRunnables = oop._asyncRunnables;
		_factory = oop._factory;
		_scheduler = oop._scheduler;
	}

	public AsyncOpBuilder WithTaskFactory(TaskFactory? factory)
	{
		_factory = factory;
		return this;
	}

	public AsyncOpBuilder WithDefaultFactory()
	{
		throw new NotImplementedException();
	}

	public AsyncOpBuilder WithCurrentFactory()
	{
		throw new NotImplementedException();
	}

	public AsyncOpBuilder WithNoFactory()
	{
		_factory = null;
		return this;
	}

	public AsyncOpBuilder WithScheduler(TaskScheduler scheduler)
	{
		_scheduler = scheduler;
		return this;
	}

	public AsyncOpBuilder WithDefaultScheduler()
	{
		throw new NotImplementedException();
	}

	public AsyncOpBuilder WithCurrentScheduler()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Attempts to select <see cref="MySingleThreadSyncContext"/>'s Task scheduler. <br/>
	/// TODO: make it actually work as intended.
	/// </summary>
	/// <returns></returns>
	public AsyncOpBuilder TryWithMyScheduler() => GetScheduler() is var scheduler ? WithScheduler(scheduler) : this;

	public AsyncOpBuilder WithNoScheduler()
	{
		_scheduler = null;
		return this;
	}

	public AsyncOpBuilder WithCancellationToken(CancellationToken token)
	{
		_token = token;
		return this;
	}

	public AsyncOpBuilder WithNoToken()
	{
		_token = null;
		return this;
	}

	public AsyncOpBuilder WithJob(IAsyncRunnable runnable) => this.ChangeRunnables(x => x.AddLast(runnable));

	public AsyncOpBuilder WithJob(Func<Task> @delegate) => this.ChangeUnCancellables(x => x.AddLast(@delegate));

	public AsyncOpBuilder WithJob(Func<CancellationToken, Task> @delegate) => this.ChangeCancellables(x => x.AddLast(@delegate));

	public AsyncOpBuilder WithJob<TResult>(Func<Task<TResult>> @delegate, out Task<TResult> result)
	{
		var source = new MyTaskSource<TResult>();
		this.WithJob(() => source.MimicResult(@delegate()));
		result = source.MyTask;
		return this;
	}

	public AsyncOpBuilder ChangeRunnables(Action<LinkedList<IAsyncRunnable>> changer)
	{
		changer(_asyncRunnables);
		return this;
	}

	public AsyncOpBuilder ChangeUnCancellables(Action<LinkedList<Func<Task>>> changer)
	{
		changer(_unCancellableTasks);
		return this;
	}

	public AsyncOpBuilder ChangeCancellables(Action<LinkedList<Func<CancellationToken, Task>>> changer)
	{
		changer(_cancellableTasks);
		return this;
	}

	public AsyncOpBuilder WithOptions(TaskCreationOptions? options)
	{
		_options = options;
		return this;
	}

	public AsyncOpBuilder WithNoOptions()
	{
		_options = null;
		return this;
	}

	private async Task RunTasks()
	{
		var queue = new LinkedList<Task>();
		var token = _token ?? default;

		foreach (var task in _asyncRunnables)
			queue.AddLast(task.RunRunnable(token));
		foreach (var task in _cancellableTasks)
			queue.AddLast(task.Invoke(token));
		foreach (var task in _unCancellableTasks)
			queue.AddLast(task.Invoke());

		await Task.WhenAll(queue);
	}

	public async Task Run()
	{
		var factory = _factory ?? new TaskFactory(_scheduler);

		await (_options is TaskCreationOptions tco ? factory.StartNew(this.RunTasks, tco) : factory.StartNew(this.RunTasks)).Unwrap();
	}
}
