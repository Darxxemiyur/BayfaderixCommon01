using System.Collections.Concurrent;

namespace Name.Bayfaderix.Darxxemiyur.Common.Async
{
	public class MySingleThreadSyncContext : SynchronizationContext, IMyUnderlyingContext
	{
		private readonly Thread _mainThread;
		private readonly MyTaskSource<TaskScheduler> _scheduler;

		public MySingleThreadSyncContext()
		{
			_handle = new(false, EventResetMode.AutoReset);
			_tasks = new();
			_scheduler = new();

			_mainThread = new Thread(Spin);
			_mainThread.Start(this);
			Post((x) => {
				_scheduler.TrySetResult(TaskScheduler.FromCurrentSynchronizationContext());
			}, null);
		}

		private readonly EventWaitHandle _handle;
		public Thread MyThread => _mainThread;
		public Task<TaskScheduler> MyTaskScheduler => _scheduler.MyTask;

		public static explicit operator TaskScheduler(MySingleThreadSyncContext context) => context.MyTaskScheduler.Result;

		public override void Post(SendOrPostCallback d, object? state)
		{
			lock (_tasks)
			{
				_tasks.Add((d, state));
				_handle.Set();
			}
		}

		//Do not make send different from post. Because intended.
		public override void Send(SendOrPostCallback d, object? state) => Post(d, state);

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