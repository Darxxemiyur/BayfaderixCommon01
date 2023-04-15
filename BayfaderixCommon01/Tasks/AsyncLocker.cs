using Name.Bayfaderix.Darxxemiyur.Extensions;

namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Fancy way of synching two parallel operations, to prevent an extreme case of parallel partial
/// data change, enabling async atomic operations to look somewhat fancy.
/// </summary>
public sealed class AsyncLocker : IDisposable
{
	private readonly SemaphoreSlim _lock;
	private readonly bool _configureAwait;
	private bool _disposedValue;
	/// <summary>
	/// Fancy way of synching two parallel operations, to prevent an extreme case of parallel partial
	/// data change, enabling async atomic operations to look somewhat fancy.
	/// </summary>
	/// <param name="configureAwait"></param>
	public AsyncLocker(bool configureAwait = false) => (_lock, _configureAwait) = (new(1, 1), configureAwait);

	public Task AsyncLock(CancellationToken token = default) => _lock.WaitAsync(token);

	public Task AsyncLock(TimeSpan time, CancellationToken token = default) => _lock.WaitAsync(time, token);

	public void Lock(CancellationToken token = default) => _lock.Wait(token);

	public void Lock(TimeSpan time, CancellationToken token = default) => _lock.Wait(time, token);

	public async Task<BlockAsyncLock> ScopeAsyncLock(CancellationToken token = default, bool configureAwait = false)
	{
		if (_disposedValue)
			throw new ObjectDisposedException(this.GetType().Name);
		await this.AsyncLock(token).ConfigureAwait(_configureAwait);
		return new BlockAsyncLock(this, configureAwait);
	}

	public BlockAsyncLock ScopeLock(bool configureAwait = false)
	{
		if (_disposedValue)
			throw new ObjectDisposedException(this.GetType().Name);
		this.Lock();
		return new BlockAsyncLock(this, configureAwait);
	}

	public Task AsyncUnlock() => MyTaskExtensions.RunOnScheduler(this.Unlock);

	public void Unlock()
	{
		if (_disposedValue)
			throw new ObjectDisposedException(this.GetType().Name);
		_lock.Release();
	}

	private void Dispose(bool disposing)
	{
		if (_disposedValue)
			return;

		if (disposing)
		{
			_lock.Dispose();
		}
		_disposedValue = true;
	}

	~AsyncLocker() => this.Dispose(false);

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
