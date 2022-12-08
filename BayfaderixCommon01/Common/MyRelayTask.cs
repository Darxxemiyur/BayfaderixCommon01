using System.Security.Cryptography;

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
		public MyRelayTask(Func<Task> work, CancellationToken token = default) => _facade = new(async () => { await work(); return false; }, token);

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
		private readonly MyTaskSource<T> _inner;
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
			await using (var _ = await _lock.BlockAsyncLock())
				_innerWork ??= SecureThingy();

			return await _innerWork;
		}

		private async Task<T> SecureThingy()
		{
			var task = _callable();

			var either = await Task.WhenAny(task, _inner.MyTask);

			await _inner.TrySetCanceledAsync();

			return await (either == task ? task : _inner.MyTask);
		}
	}

	public static class MyRelayTaskExtension
	{
		public static Task RelayAsync(this Task me, CancellationToken token) => new MyRelayTask(me, token).TheTask;
		public static async Task RelayAsync(this Task me, TimeSpan timeout, CancellationToken token = default)
		{
			using var ttokenS = new CancellationTokenSource(timeout);
			using var rtokenS = CancellationTokenSource.CreateLinkedTokenSource(token, ttokenS.Token);

			await me.RelayAsync(rtokenS.Token);
		}
		public static Task<T> RelayAsync<T>(this Task<T> me, CancellationToken token) => new MyRelayTask<T>(me, token).TheTask;
		public static async Task<T> RelayAsync<T>(this Task<T> me, TimeSpan timeout, CancellationToken token = default)
		{
			using var ttokenS = new CancellationTokenSource(timeout);
			using var rtokenS = CancellationTokenSource.CreateLinkedTokenSource(token, ttokenS.Token);

			return await me.RelayAsync(rtokenS.Token);
		}
	}
}