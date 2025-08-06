using Name.Bayfaderix.Darxxemiyur.Extensions;

namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Fancy way of synching two parallel operations, to prevent an extreme case of parallel partial data change, enabling async atomic operations to look somewhat fancy.
/// </summary>
public sealed class AsyncLocker : IDisposable
{
    private TaskCompletionSource _lock = new();
    private int _isTaken;
    private readonly bool _configureAwait;
    private bool _disposedValue;

    /// <summary>
    /// Fancy way of synching two parallel operations, to prevent an extreme case of parallel partial data change, enabling async atomic operations to look somewhat fancy.
    /// </summary>
    /// <param name="configureAwait"></param>
    public AsyncLocker(bool configureAwait = false) => _configureAwait = configureAwait;

    public async ValueTask LockAsync(CancellationToken token = default)
    {
        while (Interlocked.CompareExchange(ref _isTaken, 1, 0) == 1 && !token.IsCancellationRequested)
            await _lock.Task;

        token.ThrowIfCancellationRequested();
    }

    public async ValueTask LockAsync(TimeSpan time, CancellationToken token = default)
    {
        using var timeoutSource = new CancellationTokenSource(time);
        using var mergedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, token);

        await LockAsync(mergedSource.Token);
    }

    public async ValueTask<BlockAsyncLock> ScopeLockAsync(CancellationToken token = default, bool? configureAwait = default)
    {
        if (_disposedValue)
            throw new ObjectDisposedException(this.GetType().Name);
        await this.LockAsync(token).ConfigureAwait(configureAwait ?? _configureAwait);
        return new BlockAsyncLock(this);
    }

    public void Unlock()
    {
        if (_disposedValue)
            throw new ObjectDisposedException(this.GetType().Name);

        if (Interlocked.And(ref _isTaken, 1) == 0)
            return;

        var oldSource = Interlocked.Exchange(ref _lock, new TaskCompletionSource());
        Interlocked.Exchange(ref _isTaken, 0);
        oldSource.TrySetResult();
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        _disposedValue = true;
    }

    ~AsyncLocker() => this.Dispose(false);

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
