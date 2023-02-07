using System.Collections.Concurrent;

namespace Name.Bayfaderix.Darxxemiyur.Common.Async
{
	public class MySingleThreadSyncContext : SynchronizationContext, IMyUnderlyingContext
	{
		private readonly Thread _mainThread;

		//Use .NET's TCS because mine relies on MyTaskExtensions.
		private readonly TaskCompletionSource<TaskScheduler> _scheduler;

		private readonly TaskCompletionSource<TaskFactory> _taskFactory;

		public MySingleThreadSyncContext()
		{
			_handle = new(false, EventResetMode.AutoReset);
			_tasks = new();
			_tasksToDo = new();
			_scheduler = new();
			_taskFactory = new();
			_mainThread = new Thread(Spin);
			Post((x) => {
				var ts = TaskScheduler.FromCurrentSynchronizationContext();
				_scheduler.TrySetResult(ts);
				_taskFactory.TrySetResult(new(ts));
			}, null);
			_mainThread.Start(this);
		}

		private readonly EventWaitHandle _handle;
		public Thread MyThread => _mainThread;
		public Task<TaskScheduler> MyTaskScheduler => _scheduler.Task;
		public Task<TaskFactory> TaskFactory => _taskFactory.Task;

		public override void Post(SendOrPostCallback d, object? state)
		{
			_tasksToDo.Add((d, state));
			_handle.Set();
		}

		public override void Send(SendOrPostCallback d, object? state) => d(state);

		private readonly ConcurrentBag<(SendOrPostCallback, object?)> _tasksToDo;
		private readonly LinkedList<(SendOrPostCallback, object?)> _tasks;

		private void Spin(object contextO)
		{
			var context = contextO as MySingleThreadSyncContext;
			SetSynchronizationContext(context);

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
	}
}