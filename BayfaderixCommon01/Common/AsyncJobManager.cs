using Nito.AsyncEx;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Async job manager. Name stands for itself, but to be more specific, it just manages handling
	/// of exceptions, creation and.. whatever, I need it.
	/// </summary>
	public class AsyncJobManager : IAsyncRunnable
	{
		private readonly List<Task<Exception?>> _executingTasks;
		private readonly FIFOPTACollection<AsyncJob> _toExecuteTasks;
		private readonly Thread? _workerThread;
		private readonly bool _runsInWorkerThread;
		private readonly MyTaskSource _relay;
		private readonly CancellationToken _token;
		private readonly Func<AsyncJobManager, Exception, Task<AsyncJob>> _errorHandler;

		/// <summary>
		/// The unit of job. Start other jobs in here, do async await work in here, do whatever you
		/// want, it's up to you.
		/// </summary>
		public class AsyncJob
		{
			private readonly MyTaskSource<object> _resulter;
			public Task<object> DataResult => _resulter.MyTask;
			public Task Result => DataResult;

			/// <summary>
			/// Execution "situation" that this job wants to be in.
			/// </summary>
			public enum Type
			{
				/// <summary>
				/// Run Job in thread pool.
				/// </summary>
				Pooled,

				/// <summary>
				/// Run separate thread for job.
				/// </summary>
				UniqueThreaded,

				/// <summary>
				/// Runs in its single dedicated for jobs thread(probably gonna deprecate run in
				/// worker thread in favor of this)
				/// </summary>
				SubThreaded,

				/// <summary>
				/// Job runs in the same thread
				/// </summary>
				Inline,
			}

#if DEBUG
			public readonly Type JobType = Type.Inline;
#else
			public readonly Type JobType = Type.Inline;
#endif

			private readonly Func<CancellationToken, Task<object>> _invoke;
			private readonly CancellationToken _token;

			/// <summary>
			/// Just because handy
			/// </summary>
			/// <param name="token"></param>
			private AsyncJob(CancellationToken token = default)
			{
				_token = token;
				_resulter = new();
			}

			/// <summary>
			/// Cancellation tokens will be delivered to the supplied task.
			/// </summary>
			public AsyncJob(Func<CancellationToken, Task<object>> work, CancellationToken token = default) : this(token) => _invoke = work;

			/// <summary>
			/// Cancellation tokens will be delivered to the supplied task.
			/// </summary>
			public AsyncJob(Func<Task<object>> work, CancellationToken token = default) : this(token) => _invoke = (CancellationToken x) => work();

			/// <summary>
			/// Cancellation tokens will be delivered to the supplied task.
			/// </summary>
			public AsyncJob(Func<CancellationToken, Task> work, CancellationToken token = default) : this(token) => _invoke = async (CancellationToken x) => {
				await work(x);
				return false;
			};

			/// <summary>
			/// Cancellation tokens will be delivered to the supplied task.
			/// </summary>
			public AsyncJob(Func<Task> work, CancellationToken token = default) : this(token) => _invoke = async (CancellationToken x) => {
				await work();
				return false;
			};

			internal async Task Launch(CancellationToken managerToken)
			{
				using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(managerToken, _token);
				try
				{
					var linkekToken = linkedSource.Token;
					await _resulter.TrySetResultAsync(await _invoke(linkekToken));
				}
				catch (TaskCanceledException)
				{
					await _resulter.TrySetCanceledAsync();
					throw;
				}
				catch (Exception e)
				{
					await _resulter.TrySetExceptionAsync(e);
					throw;
				}
			}
		}

		/// <summary>
		/// Job that handles error occured in this "manager"
		/// </summary>
		/// <param name="runInWorkerThread">if true, will run in its own thread.</param>
		/// <param name="errorHandler">Handler that will generate job for handling the error.</param>
		/// <param name="token">Cancellation token that will cancell all jobs, and stop the manager</param>
		public AsyncJobManager(bool runInWorkerThread, Func<AsyncJobManager, Exception, Task<AsyncJob>> errorHandler, CancellationToken token = default)
		{
			_executingTasks = new();
			_toExecuteTasks = new();
			if (_runsInWorkerThread = runInWorkerThread)
				_workerThread = new(() => AsyncContext.Run(MyWorker));
			_relay = new();
			_errorHandler = errorHandler;
			_token = token;
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
			await _toExecuteTasks.Place(runners);

			return runners;
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
				if (job.JobType == AsyncJob.Type.Pooled)
					await Task.Run(() => job.Launch(_token));

				//Explicit external thread for each task.
				if (job.JobType == AsyncJob.Type.UniqueThreaded)
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

				//No separeate threads.
				if (job.JobType == AsyncJob.Type.Inline)
					await job.Launch(_token);
			}
			catch (Exception e)
			{
				return e;
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
					var completedTask = await Task.WhenAny(_executingTasks.Append(_toExecuteTasks.UntilPlaced(token)).ToArray()) as Task<Exception?>;
					if (completedTask == null)
					{
						_executingTasks.AddRange((await _toExecuteTasks.GetAll()).Select(SafeHandler));
						continue;
					}

					//Handle the removal of completed tasks yielded from awaiting for any
					var result = await completedTask;
					//Forward all exceptions to the stderr-ish
					if (result != null)
						await AddNew(await _errorHandler(this, result));

					//Returns false if it tries to remove 'timeout' task, and true if succeeds
					_executingTasks.Remove(completedTask);
				}
			}
			catch (Exception)
			{
				//Exit
			}
			await _relay.TrySetResultAsync();
		}

		public async Task RunRunnable()
		{
			//Implies _workerThread is not null
			if (_runsInWorkerThread)
				_workerThread.Start();
			else
				await MyWorker();

			await _relay.MyTask;
		}
	}
}