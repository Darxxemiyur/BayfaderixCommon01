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

		public AsyncLocker() => _lock = new(1, 1);

		public Task AsyncLock(CancellationToken token = default) => _lock.WaitAsync(token);

		public Task AsyncLock(TimeSpan time, CancellationToken token = default) => _lock.WaitAsync(time, token);

		public void Lock(CancellationToken token = default) => _lock.Wait(token);

		public void Lock(TimeSpan time, CancellationToken token = default) => _lock.Wait(time, token);

		public async Task<BlockAsyncLock> BlockAsyncLock(CancellationToken token = default)
		{
			await AsyncLock(token).ConfigureAwait(false);
			return new BlockAsyncLock(this);
		}

		public BlockAsyncLock BlockLock()
		{
			Lock();
			return new BlockAsyncLock(this);
		}

		public Task AsyncUnlock() => MyTaskExtensions.RunOnScheduler(Unlock);

		public void Unlock() => _lock.Release();

		public void Dispose() => ((IDisposable)_lock).Dispose();
	}
}