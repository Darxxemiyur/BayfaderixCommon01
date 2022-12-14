namespace Name.Bayfaderix.Darxxemiyur.Common
{
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