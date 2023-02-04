using Nito.AsyncEx;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Async job manager. Name stands for itself, but to be more specific, it just manages handling
	/// of exceptions, creation and.. whatever, I need it.
	/// </summary>
	//TODO: Make it schedule tasks using https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskscheduler?view=net-7.0 or something
	//TODO: Make task creation using this https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskfactory?view=net-7.0
	//TODO: AsyncJobManager looses shape, so it must be reconsidered
	//TODO: CHECK AsyncJob
	public class AsyncJobManager : IAsyncJobManager
	{
		private readonly LinkedList<Task<Exception?>> _executingTasks;
		private readonly FIFOPTACollection<AsyncJob> _toExecuteTasks;
		private readonly Thread? _workerThread;
		private readonly AsyncLocker _lock;

		/// <summary>
		/// Worker thread runner. Either sub runner, or this exact instance.
		/// </summary>
		private AsyncJobManager? _workerThreadRunner;

		private readonly MyTaskSource _relay;
		private readonly CancellationToken _token;
		private readonly Func<AsyncJobManager, Exception, Task<bool>> _errorHandler;

		/// <summary>
		/// Job that handles error occured in this "manager"
		/// </summary>
		/// <param name="runInWorkerThread">if true, will run in its own thread.</param>
		/// <param name="errorHandler">Handler that will generate job for handling the error.</param>
		/// <param name="token">Cancellation token that will cancell all jobs, and stop the manager</param>
		public AsyncJobManager(bool runInWorkerThread = false, Func<AsyncJobManager, Exception, Task<bool>>? errorHandler = null, CancellationToken token = default)
		{
			_executingTasks = new();
			_toExecuteTasks = new();
			_relay = new();
			_lock = new();
			_errorHandler = errorHandler ?? new Func<AsyncJobManager, Exception, Task<bool>>((x, y) => Task.FromResult(false));
			_token = token;
			if (!runInWorkerThread)
				return;

			_workerThread = new(x => AsyncContext.Run((Func<Task>)x)) {
				IsBackground = true
			};
			_workerThread.Priority = ThreadPriority.BelowNormal;
			_workerThreadRunner = this;
		}

		/// <summary>
		/// Returns a task that represent process of passed task, which on completion will return
		/// the completed task;
		/// </summary>
		/// <param name="runners"></param>
		/// <returns></returns>
		public async Task<AsyncJob> AddNew(AsyncJob job) => (await AddNew(new[] { job }).ConfigureAwait(false)).First();

		/// <summary>
		/// Returns a list of tasks that represent process of passed tasks, which on completion will
		/// return the completed tasks;
		/// </summary>
		/// <param name="runners"></param>
		/// <returns></returns>
		public Task<IEnumerable<AsyncJob>> AddNew(params AsyncJob[] runners) => AddNew(runners.AsEnumerable());

		/// <summary>
		/// Returns a list of tasks that represent process of passed tasks, which on completion will
		/// return the completed tasks;
		/// </summary>
		/// <param name="runners"></param>
		/// <returns></returns>
		public async Task<IEnumerable<AsyncJob>> AddNew(IEnumerable<AsyncJob> runners)
		{
			var created = runners.ToLinkedList();
			await _toExecuteTasks.PlaceLast(created).ConfigureAwait(false);

			return created;
		}

		/// <summary>
		/// Makes sure to propagate the exception back.
		/// </summary>
		/// <param name="job"></param>
		/// <returns></returns>
		private async Task<Exception?> SafeHandler(AsyncJob job)
		{
			try
			{
				// Place task in thread pool
				if (job.JobType == AsyncJobType.Pooled)
					await Task.Run(() => job.Launch(_token), job.Token).ConfigureAwait(false);

				//Explicit external thread for each task.
				if (job.JobType == AsyncJobType.UniqueThreaded)
				{
					var relay = new MyTaskSource();
					new Thread(() => AsyncContext.Run(async () => {
						try
						{
							await job.Launch(_token).ConfigureAwait(false);
							await relay.TrySetResultAsync().ConfigureAwait(false);
						}
						catch (Exception e)
						{
							await relay.TrySetExceptionAsync(e).ConfigureAwait(false);
						}
					})) {
						IsBackground = !true
					}.Start();
					await relay.MyTask.ConfigureAwait(false);
				}

				// Run in subthread of this job manager
				if (job.JobType == AsyncJobType.SubThreaded)
				{
					await using (var _ = await _lock.BlockAsyncLock().ConfigureAwait(false))
						if (_workerThreadRunner == null)
							await AddNew(new AsyncJob(_workerThreadRunner = new AsyncJobManager(true, _errorHandler, _token), AsyncJobType.UniqueThreaded, _token)).ConfigureAwait(false);

					//Yes, job to launch a job. Cry about it.
					await (await _workerThreadRunner.AddNew(new AsyncJob(() => job.Launch(_token))).ConfigureAwait(false)).Result.ConfigureAwait(false);
				}

				//Run in the same thread and task scheduler.
				if (job.JobType == AsyncJobType.Inline)
					await job.Launch(_token).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				return new AsyncJobManagerException(e);
			}

			return null;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		private async Task MyWorker()
		{
			var token = _token;
			try
			{
				while (!token.IsCancellationRequested)
				{
					//Make _executingTasks have a list of Node<Task(the proxy task)>, while the Task would complete once parent task completes, the visible to node task type is generic, and should be upcasted to Task<((Node(the node itself),Task<Exception?>))
					//HOWEVER THAT IS LIKELY NOT THE CAUSE OF MEMORY LEAK
					//https://github.com/dotnet/BenchmarkDotNet

					//Wait for any task to complete in the list
					var completedTask = await Task.WhenAny(_executingTasks.ToMyThingy().Append(_toExecuteTasks.UntilPlaced(token))).ConfigureAwait(false) as Task<LinkedListNode<Task<Exception?>>>;
					if (completedTask == null)
					{
						foreach (var newTask in (await _toExecuteTasks.GetAll().ConfigureAwait(false)).Select(SafeHandler))
							_executingTasks.AddLast(newTask);

						continue;
					}
					//Handle the removal of completed tasks yielded from awaiting for any
					var node = await completedTask.ConfigureAwait(false);
					var result = await node.Value.ConfigureAwait(false);

					_executingTasks.Remove(node);

					if (result != null && !await _errorHandler(this, result).ConfigureAwait(false))
						throw new AsyncJobManagerException(result);
				}
				await _relay.TrySetResultAsync().ConfigureAwait(false);
			}
			catch (Exception e)
			{
				await _relay.TrySetExceptionAsync(e).ConfigureAwait(false);
			}
		}

		public async Task RunRunnable(CancellationToken token = default)
		{
			//Implies _workerThread is not null
			if (_workerThread != null)
				_workerThread.Start(MyWorker);
			else
				await MyWorker().ConfigureAwait(false);

			await _relay.MyTask.ConfigureAwait(false);
		}
	}
}