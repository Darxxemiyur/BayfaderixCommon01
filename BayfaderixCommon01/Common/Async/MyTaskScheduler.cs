using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Name.Bayfaderix.Darxxemiyur.Common.Async
{
	public sealed class MyTaskScheduler : TaskScheduler
	{
		private MySynchronizationContext _context;
		public MyTaskScheduler(MySynchronizationContext context)
		{
			_context = context;
		}

		protected override IEnumerable<Task>? GetScheduledTasks() => throw new NotImplementedException();
		protected override void QueueTask(Task task)
		{
			_context.Post(x => base.TryExecuteTask(x as Task), task);
		}
		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;
	}
}
