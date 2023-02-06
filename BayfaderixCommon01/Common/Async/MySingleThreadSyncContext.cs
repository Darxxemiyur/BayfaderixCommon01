using System.Collections.Concurrent;

namespace Name.Bayfaderix.Darxxemiyur.Common.Async
{
	public class MySingleThreadSyncContext : SynchronizationContext, IMyUnderlyingContext
	{
		private readonly Thread _mainThread;

		//Use .NET's TCS because mine relies on MyTaskExtensions.
		private readonly TaskCompletionSource<TaskScheduler> _scheduler;

		private readonly Task<TaskFactory> _taskFactory;

		public MySingleThreadSyncContext()
		{
			_handle = new(false, EventResetMode.AutoReset);
			_tasks = new();
			_scheduler = new();
			_taskFactory = _scheduler.Task.ContinueWith(x => new TaskFactory(x.Result));
			_mainThread = new Thread(Spin);
			Post((x) => _scheduler.TrySetResult(TaskScheduler.FromCurrentSynchronizationContext()), null);
			_mainThread.Start(this);
		}

		private readonly EventWaitHandle _handle;
		public Thread MyThread => _mainThread;
		public Task<TaskScheduler> MyTaskScheduler => _scheduler.Task;
		public Task<TaskFactory> TaskFactory => _taskFactory;

		public override void Post(SendOrPostCallback d, object? state)
		{
			lock (_tasks)
			{
				_tasks.Add((d, state));
				_handle.Set();
			}
		}

		public override void Send(SendOrPostCallback d, object? state) => d(state);

		private readonly ConcurrentBag<(SendOrPostCallback, object?)> _tasks;

		private void Spin(object contextO)
		{
			var context = contextO as MySingleThreadSyncContext;
			SetSynchronizationContext(context);

			while (true)
			{
				SendOrPostCallback d = null;
				object arg = null;
				lock (_tasks)
					if (_tasks.TryTake(out var item))
						(d, arg) = item;
				if (d != null)
					d(arg);
				else
					_handle.WaitOne();
			}
		}
	}
}