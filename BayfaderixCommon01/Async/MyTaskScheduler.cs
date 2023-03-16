namespace Name.Bayfaderix.Darxxemiyur.Common.Async
{
	/// <summary>
	/// TODO: find use, if none, remove later.
	/// </summary>
	internal sealed class MyTaskScheduler : TaskScheduler
	{
		private readonly MySingleThreadSyncContext _context;

		public MyTaskScheduler(MySingleThreadSyncContext context)
		{
			_context = context;
		}

		protected override IEnumerable<Task>? GetScheduledTasks() => throw new NotImplementedException();

		protected override void QueueTask(Task task)
		{
			_context.Post(x => base.TryExecuteTask(task), null);
		}

		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => Current == this && this.TryExecuteTask(task);
	}
}
