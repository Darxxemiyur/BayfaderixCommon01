using Name.Bayfaderix.Darxxemiyur.Tasks;

namespace Name.Bayfaderix.Darxxemiyur.Async;

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

	public static AsyncOpBuilder Get => new();
	private readonly LinkedList<Action> _unCancellableActions;
	private readonly LinkedList<Action<CancellationToken>> _cancellableActions;
	private readonly LinkedList<Func<Task>> _unCancellableTasks;
	private readonly LinkedList<Func<CancellationToken, Task>> _cancellableTasks;
	private readonly LinkedList<IAsyncRunnable> _asyncRunnables;
	private CancellationToken? _token;
	private TaskCreationOptions? _crOptions;
	private TaskContinuationOptions? _cnOptions;
	private TaskFactory? _factory;
	private TaskScheduler? _scheduler;
	private bool _ca;

	public AsyncOpBuilder()
	{
		_unCancellableActions = new();
		_cancellableActions = new();
		_unCancellableTasks = new();
		_cancellableTasks = new();
		_asyncRunnables = new();
	}

	public AsyncOpBuilder(AsyncOpBuilder oop)
	{
		_unCancellableActions = oop._unCancellableActions;
		_cancellableActions = oop._cancellableActions;
		_unCancellableTasks = oop._unCancellableTasks;
		_cancellableTasks = oop._cancellableTasks;
		_asyncRunnables = oop._asyncRunnables;
		_factory = oop._factory;
		_scheduler = oop._scheduler;
		_crOptions = oop._crOptions;
		_cnOptions = oop._cnOptions;
		_ca = oop._ca;
	}

	public AsyncOpBuilder WithTaskFactory(TaskFactory? factory)
	{
		_factory = factory;
		return this;
	}

	public AsyncOpBuilder WithDefaultFactory() => throw new NotImplementedException();

	public AsyncOpBuilder WithCurrentFactory() => throw new NotImplementedException();

	public AsyncOpBuilder WithNoFactory()
	{
		_factory = null;
		return this;
	}

	public AsyncOpBuilder WithScheduler(TaskScheduler? scheduler)
	{
		_scheduler = scheduler;
		return this;
	}

	public AsyncOpBuilder WithScheduler(IMyUnderlyingContext? context) => this.WithScheduler(context?.MyTaskScheduler);

	public AsyncOpBuilder WithDefaultScheduler() => this.WithScheduler(GetScheduler());

	public AsyncOpBuilder WithCurrentScheduler() => this.WithScheduler(TaskScheduler.Current);

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

	public AsyncOpBuilder WithCancellationToken(CancellationToken? token)
	{
		_token = token;
		return this;
	}

	public AsyncOpBuilder WithNoToken()
	{
		_token = null;
		return this;
	}

	public AsyncOpBuilder WithJob(Action action) => this.ChangeUnCancellablesAc(x => x.AddLast(action));

	public AsyncOpBuilder WithJob(Action<CancellationToken> action) => this.ChangeCancellablesAc(x => x.AddLast(action));

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

	public AsyncOpBuilder ChangeUnCancellablesAc(Action<LinkedList<Action>> changer)
	{
		changer(_unCancellableActions);
		return this;
	}

	public AsyncOpBuilder ChangeCancellablesAc(Action<LinkedList<Action<CancellationToken>>> changer)
	{
		changer(_cancellableActions);
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

	public AsyncOpBuilder WithCreationOptions(TaskCreationOptions? options)
	{
		_crOptions = options;
		return this;
	}

	public AsyncOpBuilder WithContinuationOptions(TaskContinuationOptions? options)
	{
		_cnOptions = options;
		return this;
	}

	public AsyncOpBuilder WithNoCreationOptions() => this.WithCreationOptions(null);

	public AsyncOpBuilder WithNoContinuationOptions() => this.WithContinuationOptions(null);

	public AsyncOpBuilder WithConfigureAwait(bool configureAwait)
	{
		_ca = configureAwait;
		return this;
	}

	public async Task<LinkedList<Task>> Run(CancellationToken token = default)
	{
		var queue = new LinkedList<Task>();
		var innerToken = _token ?? default;
		using var ts = CancellationTokenSource.CreateLinkedTokenSource(innerToken, token);
		var tokenU = ts.Token;
		var sched = _factory?.Scheduler ?? _scheduler;
		var cropts = (_crOptions ?? TaskCreationOptions.None) | (_factory?.CreationOptions ?? TaskCreationOptions.None);
		var cnopts = (_cnOptions ?? TaskContinuationOptions.None) | (_factory?.ContinuationOptions ?? TaskContinuationOptions.None);
		var factory = new TaskFactory(tokenU, cropts, cnopts, sched);

		foreach (var task in _asyncRunnables)
			queue.AddLast(factory.StartNew(() => task.RunRunnable(tokenU), tokenU).Unwrap());
		foreach (var task in _cancellableTasks)
			queue.AddLast(factory.StartNew(() => task.Invoke(tokenU), tokenU).Unwrap());
		foreach (var task in _unCancellableTasks)
			queue.AddLast(factory.StartNew(() => task.Invoke(), tokenU).Unwrap());
		foreach (var task in _cancellableActions)
			queue.AddLast(factory.StartNew(() => task.Invoke(tokenU), tokenU));
		foreach (var task in _unCancellableActions)
			queue.AddLast(factory.StartNew(() => task.Invoke(), tokenU));

		await factory.StartNew(() => Task.WhenAll(queue)).Unwrap().ConfigureAwait(_ca);
		return queue;
	}
}
