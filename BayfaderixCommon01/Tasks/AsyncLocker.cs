using Name.Bayfaderix.Darxxemiyur.Common.Extensions;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Fancy way of synching two parallel operations, to prevent an extreme case of parallel
	/// partial data change, enabling async atomic operations to look somewhat fancy.
	/// </summary>
	public sealed class AsyncLocker : IDisposable
	{
		private readonly SemaphoreSlim _lock;
		private readonly bool _configureAwait;

		public AsyncLocker(bool configureAwait = false) => (_lock, _configureAwait) = (new(1, 1), configureAwait);

		public Task AsyncLock(CancellationToken token = default) => _lock.WaitAsync(token);

		public Task AsyncLock(TimeSpan time, CancellationToken token = default) => _lock.WaitAsync(time, token);

		public void Lock(CancellationToken token = default) => _lock.Wait(token);

		public void Lock(TimeSpan time, CancellationToken token = default) => _lock.Wait(time, token);

		public async Task<BlockAsyncLock> BlockAsyncLock(CancellationToken token = default, bool configureAwait = false)
		{
			await AsyncLock(token).ConfigureAwait(_configureAwait);
			return new BlockAsyncLock(this, configureAwait);
		}

		public BlockAsyncLock BlockLock(bool configureAwait = false)
		{
			Lock();
			return new BlockAsyncLock(this, configureAwait);
		}

		public Task AsyncUnlock() => MyTaskExtensions.RunOnScheduler(Unlock);

		public void Unlock() => _lock.Release();

		public void Dispose() => ((IDisposable)_lock).Dispose();
	}
}