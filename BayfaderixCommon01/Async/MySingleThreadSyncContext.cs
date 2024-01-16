using System.Collections.Concurrent;

namespace Name.Bayfaderix.Darxxemiyur.Async;

public class MySingleThreadSyncContext : SynchronizationContext, IMyUnderlyingContext, IDisposable
{
	public CancellationTokenSource Cancellation => _inner.Cancellation;

	private readonly MySingleThreadSyncContextInner _inner;

	public MySingleThreadSyncContext(ThreadPriority threadPriority = ThreadPriority.Normal) => _inner = new MySingleThreadSyncContextInner(threadPriority);

	public Thread MyThread => _inner.MyThread;

	public Task<TaskScheduler> MyTaskSchedulerPromise => _inner.MyTaskSchedulerPromise;

	public Task<TaskFactory> MyTaskFactoryPromise => _inner.MyTaskFactoryPromise;

	public TaskScheduler? MyTaskScheduler => _inner.MyTaskScheduler;

	public TaskFactory? MyTaskFactory => _inner.MyTaskFactory;

	public SynchronizationContext ThisContext => _inner.ThisContext;

	public override void Post(SendOrPostCallback d, object? state) => _inner.Post(d, state);

	public override void Send(SendOrPostCallback d, object? state) => _inner.Send(d, state);

	public Task Place(BatchAsyncOpBuilder asyncOp) => _inner.Place(asyncOp);

	private bool _disposedValue;

	protected virtual void Dispose(bool disposing)
	{
		if (_disposedValue)
			return;

		if (disposing)
			_inner.Dispose();

		_disposedValue = true;
	}

	~MySingleThreadSyncContext() => this.Dispose(disposing: false);

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

internal class MySingleThreadSyncContextInner : SynchronizationContext, IMyUnderlyingContext, IDisposable
{
	private readonly Thread _mainThread;

	public CancellationTokenSource Cancellation
	{
		get;
	}

	public MySingleThreadSyncContextInner(ThreadPriority threadPriority = ThreadPriority.Normal)
	{
		Cancellation = new CancellationTokenSource();
		_handle = new(false, EventResetMode.AutoReset);
		Cancellation.Token.Register(() => _handle.Set());
		_tasks = new();
		_myTaskSchedulerSource = new();
		_myTaskFactorySource = new();
		_tasksToDo = new();
		_mainThread = new Thread(this.Spin)
		{
			IsBackground = true,
			Priority = threadPriority
		};
	}

	private readonly EventWaitHandle _handle;
	public Thread MyThread => _mainThread;

	private readonly TaskCompletionSource<TaskScheduler> _myTaskSchedulerSource;

	public Task<TaskScheduler> MyTaskSchedulerPromise => _myTaskSchedulerSource.Task;

	private readonly TaskCompletionSource<TaskFactory> _myTaskFactorySource;

	public Task<TaskFactory> MyTaskFactoryPromise => _myTaskFactorySource.Task;

	public TaskScheduler? MyTaskScheduler
	{
		get;
		private set;
	}

	public TaskFactory? MyTaskFactory
	{
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
	private bool _disposedValue;
	private void Spin()
	{
		var ts = TaskScheduler.FromCurrentSynchronizationContext();
		_myTaskSchedulerSource.TrySetResult(MyTaskScheduler = ts);
		_myTaskFactorySource.TrySetResult(MyTaskFactory = new TaskFactory(ts));

		SetSynchronizationContext(this);
		while (!Cancellation.IsCancellationRequested)
		{
			while (_tasksToDo.TryTake(out var item))
				_tasks.AddLast(item);

			foreach ((var d, var o) in _tasks)
				d?.Invoke(o);

			_tasks.Clear();
			if (_tasksToDo.IsEmpty)
				_handle.WaitOne();
		}
	}

	public Task Place(BatchAsyncOpBuilder asyncOp) => asyncOp.WithScheduler(MyTaskScheduler).Start();

	protected virtual void Dispose(bool disposing)
	{
		if (_disposedValue || !disposing)
			return;

		Cancellation.Cancel();
		_handle.Set();
		Cancellation.Dispose();
		_handle.Dispose();

		_disposedValue = true;
	}

	~MySingleThreadSyncContextInner() => this.Dispose(disposing: false);

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
