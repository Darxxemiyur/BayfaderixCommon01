namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Presents GC removable ISmartAsyncRunnable
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class AsyncRunnableFinilizable<T> : IDisposable, IAsyncDisposable, ISmartAsyncRunnable where T : ISmartAsyncRunnable
{
	private readonly T _runnable;
	private bool _disposedValue;

	public MyTaskSource<object> ExposedObject => _runnable.ExposedObject;

	public AsyncRunnableFinilizable(T instance) => _runnable = instance;

	public async ValueTask DisposeAsync()
	{
		if (_disposedValue)
			return;
		_disposedValue = true;
		GC.SuppressFinalize(this);

		if (_runnable is not IAsyncDisposable adr)
			return;

		await adr.DisposeAsync();
	}

	public Task RunRunnable(CancellationToken token = default) => _runnable.RunRunnable(token);

	public Task StopRunnable(CancellationToken token = default) => _runnable.StopRunnable(token);

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "Following the snippet IDisposable interface pattern.")]
	private void Dispose(bool disposing)
	{
		if (_disposedValue)
			return;
		_disposedValue = true;

		if (_runnable is IDisposable dr)
			dr.Dispose();
	}

	~AsyncRunnableFinilizable() => this.Dispose(disposing: false);

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public Task<bool> IsRunning(CancellationToken token = default) => _runnable.IsRunning(token);
}
