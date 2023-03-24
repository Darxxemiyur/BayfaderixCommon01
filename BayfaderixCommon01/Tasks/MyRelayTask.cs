namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Acts as a relay that allows to stop awaiting of non-cancellable task, almost seamlessly.
	/// </summary>
	//Soon [Obsolete("Use .WaitAsync() instead.")]
	public sealed class MyRelayTask
	{
		private readonly MyRelayTask<bool> _facade;

		/// <summary>
		/// </summary>
		/// <param name="work">The task to await</param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Task work, CancellationToken token = default, bool configureAwait = false) : this(() => work, token, configureAwait)
		{
		}

		/// <summary>
		/// </summary>
		/// <param name="work">
		/// Delegate that starts the task. Be aware that it starts only after accessing the TheTask property
		/// </param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Func<Task> work, CancellationToken token = default, bool configureAwait = false) => _facade = new(async () => { await work().ConfigureAwait(configureAwait); return false; }, token, configureAwait);

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
		private readonly bool _configureAwait;

		/// <summary>
		/// The cancellable relay task.
		/// </summary>
		public Task<T> TheTask => this.Encapsulate();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		// _callable is not null because this private constructor is called on all public constructors.
		private MyRelayTask(CancellationToken token = default, bool configureAwait = false)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			_inner = new(token);
			_lock = new();
			_configureAwait = configureAwait;
		}

		/// <summary>
		/// </summary>
		/// <param name="work">The task to await</param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Task<T> work, CancellationToken token = default, bool configureAwait = false) : this(() => work, token, configureAwait) { }

		/// <summary>
		/// </summary>
		/// <param name="work">
		/// Delegate that starts the task. Be aware that it starts only after accessing the TheTask property
		/// </param>
		/// <param name="token">Cancellation token to cancel the proxy task</param>
		public MyRelayTask(Func<Task<T>> work, CancellationToken token = default, bool configureAwait = false) : this(token, configureAwait) => _callable = work;

		private async Task<T> Encapsulate()
		{
			await using (var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
				_innerWork ??= this.SecureThingy();

			return await _innerWork.ConfigureAwait(_configureAwait);
		}

		private async Task<T> SecureThingy()
		{
			var task = _callable();

			var either = await Task.WhenAny(task, _inner.MyTask).ConfigureAwait(_configureAwait);

			try
			{
				if (either == task)
					await _inner.TrySetResultAsync().ConfigureAwait(_configureAwait);
				else
					await _inner.MyTask.ConfigureAwait(_configureAwait);

				return await task.ConfigureAwait(_configureAwait);
			}
			catch (Exception e)
			{
				throw new MyRelayTaskException(e);
			}
		}
	}
}
