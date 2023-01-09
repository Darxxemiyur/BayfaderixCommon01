using Nito.AsyncEx;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Async job manager. Name stands for itself, but to be more specific, it just manages handling
	/// of exceptions, creation and.. whatever, I need it.
	/// </summary>
	public class AsyncJobManager : IAsyncJobManager
	{
		private readonly LinkedList<Task<Exception?>> _executingTasks;
		private readonly FIFOPTACollection<AsyncJob> _toExecuteTasks;
		private readonly Thread? _workerThread;

		/// <summary>
		/// Worker thread runner. Either sub runner, or this exact instance.
		/// </summary>
		private AsyncJobManager _workerThreadRunner;

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
			_errorHandler = errorHandler ?? new Func<AsyncJobManager, Exception, Task<bool>>((x, y) => Task.FromResult(false));
			_token = token;
			if (!runInWorkerThread)
				return;

			_workerThread = new(() => AsyncContext.Run(MyWorker));
			_workerThreadRunner = this;
		}

		/// <summary>
		/// Returns a task that represent process of passed task, which on completion will return
		/// the completed task;
		/// </summary>
		/// <param name="runners"></param>
		/// <returns></returns>
		public async Task<AsyncJob> AddNew(AsyncJob job) => (await AddNew(new[] { job })).First();

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
			await _toExecuteTasks.PlaceLast(created);

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
					await Task.Run(() => job.Launch(_token));

				//Explicit external thread for each task.
				if (job.JobType == AsyncJobType.UniqueThreaded)
				{
					var relay = new MyTaskSource();
					new Thread(() => AsyncContext.Run(async () => {
						try
						{
							await job.Launch(_token);
							await relay.TrySetResultAsync();
						}
						catch (Exception e)
						{
							await relay.TrySetExceptionAsync(e);
						}
					})).Start();
					await relay.MyTask;
				}

				// Run in subthread of this job manager
				if (job.JobType == AsyncJobType.SubThreaded)
				{
					if (_workerThreadRunner == null)
					{
						_workerThreadRunner = new AsyncJobManager(true, _errorHandler, _token);
						await AddNew(new AsyncJob(_workerThreadRunner, AsyncJobType.UniqueThreaded, _token));
					}
					//Yes, job to launch a job. Cry about it.
					await (await _workerThreadRunner.AddNew(new AsyncJob(() => job.Launch(_token)))).Result;
				}

				//Run in the same thread as this job manager.
				if (job.JobType == AsyncJobType.Inline)
					await job.Launch(_token);
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
					//Wait for any task to complete in the list
					var completedTask = await Task.WhenAny(_executingTasks.ToMyThingy().Append(_toExecuteTasks.UntilPlaced(token))) as Task<LinkedListNode<Task<Exception?>>>;
					if (completedTask == null)
					{
						foreach (var newTask in (await _toExecuteTasks.GetAll()).Select(SafeHandler))
							_executingTasks.AddLast(newTask);

						continue;
					}
					//Handle the removal of completed tasks yielded from awaiting for any
					var node = await completedTask;
					var result = await node.Value;

					//Returns false if it tries to remove 'timeout' task, and true if succeeds
					_executingTasks.Remove(node);

					if (result != null && !await _errorHandler(this, result))
						throw new AsyncJobManagerException(result);
				}
				await _relay.TrySetResultAsync();
			}
			catch (Exception e)
			{
				await _relay.TrySetExceptionAsync(e);
			}
		}

		public async Task RunRunnable(CancellationToken token = default)
		{
			//Implies _workerThread is not null
			if (_workerThread != null)
				_workerThread.Start();
			else
				await MyWorker();

			await _relay.MyTask;
		}
	}
}