using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Name.Bayfaderix.Darxxemiyur.Common.Async
{
	public class MySynchronizationContext : SynchronizationContext
	{

		private readonly Thread _mainThread;
		private long _jobs;
		[ContextStatic] private string _msg;
		public MySynchronizationContext()
		{
			_msg = "Small ";
			_handle = new(false, EventResetMode.AutoReset);
			_tasks = new();
			_jobs = -1;

			_mainThread = new Thread(Spin);
			_mainThread.Start(this);
		}
		private EventWaitHandle _handle;
		public string GetMyMsg() => _msg;
		public override void Post(SendOrPostCallback d, object? state)
		{
			lock (_tasks)
			{
				_tasks.Add((d, state));
				if (_jobs == -1)
					_jobs = 0;
				Interlocked.Increment(ref _jobs);
				_handle.Set();
			}
		}
		public override void Send(SendOrPostCallback d, object? state)
		{
			lock (_tasks)
			{
				_tasks.Add((d, state));
				if (_jobs == -1)
					_jobs = 0;
				Interlocked.Increment(ref _jobs);
				_handle.Set();
			}
		}

		private readonly ConcurrentBag<(SendOrPostCallback, object?)> _tasks;

		private void Spin(object contextO)
		{
			var context = contextO as MySynchronizationContext;

			SetSynchronizationContext(context);
			while (true)
			{
				SendOrPostCallback d = null;
				object arg = null;
				lock (_tasks)
					if (_tasks.TryTake(out var item))
						(d, arg) = item;

				if (d != null)
				{
					d(arg);
					Interlocked.Decrement(ref _jobs);
				}
				else
				{
					_handle.WaitOne();
				}
			}

		}
	}
}
