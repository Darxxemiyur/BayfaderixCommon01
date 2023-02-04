namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Acts as a relay that allows to stop awaiting of non-cancellable task, almost seamlessly.
	/// </summary>
	//Soon [Obsolete("Use .WaitAsync() instead.")]
	public class MyRelayTask
	{
		private readonly MyRelayTask<bool> _facade;

		/// <summary>
		/// </summary>
		/// <param name="work">The task to await</param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Task work, CancellationToken token = default) : this(() => work, token)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="work">
		/// Delegate that starts the task. Be aware that it starts only after accessing the TheTask property
		/// </param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Func<Task> work, CancellationToken token = default) => _facade = new(async () => { await work().ConfigureAwait(false); return false; }, token);

		/// <summary>
		/// The cancellable relay task.
		/// </summary>
		public Task TheTask => _facade.TheTask;
	}

	/// <summary>
	/// Acts as a relay that allows to stop awaiting of non-cancellable task, almost seamlessly.
	/// </summary>
	//Soon [Obsolete("Use .WaitAsync() instead.")]
	public class MyRelayTask<T>
	{
		private readonly MyTaskSource _inner;
		private Task<T> _innerWork;
		private readonly Func<Task<T>> _callable;
		private readonly AsyncLocker _lock;

		/// <summary>
		/// The cancellable relay task.
		/// </summary>
		public Task<T> TheTask => Encapsulate();

		private MyRelayTask(CancellationToken token = default)
		{
			_inner = new(token);
			_lock = new();
		}

		/// <summary>
		/// </summary>
		/// <param name="work">The task to await</param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Task<T> work, CancellationToken token = default) : this(() => work, token) { }

		/// <summary>
		/// </summary>
		/// <param name="work">
		/// Delegate that starts the task. Be aware that it starts only after accessing the TheTask property
		/// </param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Func<Task<T>> work, CancellationToken token = default) : this(token) => _callable = work;

		private async Task<T> Encapsulate()
		{
			await using (var _ = await _lock.BlockAsyncLock().ConfigureAwait(false))
				_innerWork ??= SecureThingy();

			return await _innerWork.ConfigureAwait(false);
		}

		private async Task<T> SecureThingy()
		{
			var task = _callable();

			var either = await Task.WhenAny(task, _inner.MyTask).ConfigureAwait(false);

			try
			{
				if (either == task)
					await _inner.TrySetResultAsync().ConfigureAwait(false);
				else
					await _inner.MyTask.ConfigureAwait(false);

				return await task.ConfigureAwait(false);
			}
			catch (TaskCanceledException e)
			{
				throw new MyRelayTaskException(e);
			}
		}
	}
}