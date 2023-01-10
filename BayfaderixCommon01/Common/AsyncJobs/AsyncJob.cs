namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// The unit of job. Start other jobs from here, do async await work in here, do whatever you
	/// want, it's up to you.
	/// </summary>
	//TODO: Make task creation using this https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.taskfactory?view=net-7.0
	//TODO: AsyncJob looses shape, so it must be reconsidered
	//TODO: CHECK AsyncJobManager
	public class AsyncJob
	{
		private readonly MyTaskSource<object> _resulter;
		public Task<object> DataResult => _resulter.MyTask;
		public Task Result => DataResult;

		public readonly AsyncJobType JobType;

		private readonly Func<CancellationToken, Task<object>> _invoke;
		private readonly CancellationToken _token;
		public CancellationToken Token => _token;

		/// <summary>
		/// Just because handy
		/// </summary>
		/// <param name="token"></param>
		private AsyncJob(AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default)
		{
			_token = token;
			JobType = jobType;
			_resulter = new();
		}

		/// <summary>
		/// Cancellation tokens will be delivered to the supplied task.
		/// </summary>
		public AsyncJob(Func<CancellationToken, Task<object>> work, AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default) : this(jobType, token) => _invoke = work;

		/// <summary>
		/// Cancellation tokens will be delivered to the supplied task.
		/// </summary>
		public AsyncJob(Func<Task<object>> work, AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default) : this(jobType, token) => _invoke = (CancellationToken x) => work();

		/// <summary>
		/// Cancellation tokens will be delivered to the supplied task.
		/// </summary>
		public AsyncJob(Func<CancellationToken, Task> work, AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default) : this(jobType, token) => _invoke = async (CancellationToken x) => {
			await work(x);
			return false;
		};

		/// <summary>
		/// Cancellation tokens will be delivered to the supplied task.
		/// </summary>
		public AsyncJob(Func<Task> work, AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default) : this(jobType, token) => _invoke = async (CancellationToken x) => {
			await work();
			return false;
		};

		/// <summary>
		/// Cancellation tokens will be delivered to the supplied task.
		/// </summary>
		public AsyncJob(Func<Task<IAsyncRunnable>> work, AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default) : this(jobType, token) => _invoke = async (CancellationToken x) => {
			var runnable = await work();
			await runnable.RunRunnable(x);
			return false;
		};

		/// <summary>
		/// Cancellation tokens will be delivered to the supplied task.
		/// </summary>
		public AsyncJob(IAsyncRunnable work, AsyncJobType jobType = AsyncJobType.Inline, CancellationToken token = default) : this(jobType, token) => _invoke = async (CancellationToken x) => {
			await work.RunRunnable(x);
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
			catch (Exception e)
			{
				var exc = new AsyncJobException(e);
				await _resulter.TrySetExceptionAsync(exc);
				throw exc;
			}
		}
	}
}