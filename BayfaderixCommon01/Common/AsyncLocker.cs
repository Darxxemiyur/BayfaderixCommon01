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
			await AsyncLock(token);
			return new BlockAsyncLock(this);
		}

		public BlockAsyncLock BlockLock()
		{
			Lock();
			return new BlockAsyncLock(this);
		}

		public Task AsyncUnlock() => Task.Run(Unlock);

		public void Unlock() => _lock.Release();

		public void Dispose() => ((IDisposable)_lock).Dispose();
	}

	/// <summary>
	/// Block async lock, allows to forget about iffs with exceptions scope and etc. Used in using
	/// statement. Forgetting about it also isn't critical, as if it gets gc collected, it unlocks itself.
	/// </summary>
	public sealed class BlockAsyncLock : IDisposable, IAsyncDisposable
	{
		private readonly AsyncLocker _lock;
		private bool _unlocked;

		public BlockAsyncLock(AsyncLocker tlock) => _lock = tlock;

		~BlockAsyncLock() => TryToRelease();

		private void TryToRelease()
		{
			if (_unlocked)
				return;

			_unlocked = true;
			_lock.Unlock();
		}

		private async Task TryToReleaseAsync()
		{
			if (_unlocked)
				return;

			_unlocked = true;
			await _lock.AsyncUnlock();
		}

		public void Dispose() => TryToRelease();

		public ValueTask DisposeAsync() => new(TryToReleaseAsync());
	}
}