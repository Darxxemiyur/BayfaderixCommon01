using System.Collections.Concurrent;

namespace Name.Bayfaderix.Darxxemiyur.Async;

public class MySingleThreadSyncContext : SynchronizationContext, IMyUnderlyingContext
{
	private readonly Thread _mainThread;

	public MySingleThreadSyncContext(ThreadPriority threadPriority = ThreadPriority.Normal)
	{
		_handle = new(false, EventResetMode.AutoReset);
		_tasks = new();
		_myTaskSchedulerSource = new();
		_myTaskFactorySource = new();
		_tasksToDo = new();
		_mainThread = new Thread(this.Spin) {
			IsBackground = true,
			Priority = threadPriority
		};
		this.Post((x) => {
			var ts = TaskScheduler.FromCurrentSynchronizationContext();
			var tf = new TaskFactory(ts);
			MyTaskScheduler = ts;
			_myTaskSchedulerSource.TrySetResult(ts);
			MyTaskFactory = tf;
			_myTaskFactorySource.TrySetResult(tf);
		}, null);
		_mainThread.Start(this);
	}

	private readonly EventWaitHandle _handle;
	public Thread MyThread => _mainThread;

	private readonly TaskCompletionSource<TaskScheduler> _myTaskSchedulerSource;

	public Task<TaskScheduler> MyTaskSchedulerPromise => _myTaskSchedulerSource.Task;

	private readonly TaskCompletionSource<TaskFactory> _myTaskFactorySource;

	public Task<TaskFactory> MyTaskFactoryPromise => _myTaskFactorySource.Task;

	public TaskScheduler? MyTaskScheduler {
		get;
		private set;
	}

	public TaskFactory? MyTaskFactory {
		get;
		private set;
	}

	public SynchronizationContext ThisContext => this;

	public override void Post(SendOrPostCallback d, object? state)
	{
		_tasksToDo.Add((d, state));
		_handle.Set();
	}

	public override void Send(SendOrPostCallback d, object? state) => d(state);

	private readonly ConcurrentBag<(SendOrPostCallback, object?)> _tasksToDo;
	private readonly LinkedList<(SendOrPostCallback, object?)> _tasks;

	private void Spin(object? context)
	{
		SetSynchronizationContext(context as MySingleThreadSyncContext);
		while (true)
		{
			while (_tasksToDo.TryTake(out var item))
				_tasks.AddLast(item);

			foreach ((var d, var o) in _tasks)
				d?.Invoke(o);

			_tasks.Clear();
			_handle.WaitOne();
		}
	}

	public async Task Place(AsyncOpBuilder asyncOp)
	{
	}
}
