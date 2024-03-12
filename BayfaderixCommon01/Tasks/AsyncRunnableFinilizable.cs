namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Presents GC removable ISmartAsyncRunnable
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class AsyncRunnableFinilizable<T, TRunnable> : IDisposable, IAsyncDisposable, ISmartAsyncRunnable where T : ISmartAsyncRunnable<TRunnable>
{
	private readonly T _runnable;
	private bool _disposedValue;
	private bool _ca;

	public Task<TRunnable> ExposedObject => _runnable.ExposedObject;

	public AsyncRunnableFinilizable(T instance) => _runnable = instance;

	public AsyncRunnableFinilizable<T, TRunnable> ConfigureAwait(bool ca)
	{
		_ca = ca;
		return this;
	}

	public async ValueTask DisposeAsync()
	{
		if (_disposedValue)
			return;

		if (_runnable is not IAsyncDisposable adr)
			return;

		_disposedValue = true;
		GC.SuppressFinalize(this);

		await adr.DisposeAsync().ConfigureAwait(_ca);
	}

	public Task RunRunnable(CancellationToken token = default) => _runnable.RunRunnable(token);

	public Task StopRunnable(CancellationToken token = default) => _runnable.StopRunnable(token);

	private const string bruh = "It's practically an equivivalent to call from Dispose, but it also checks if it is even worth calling.";

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3971:\"GC.SuppressFinalize\" should not be called", Justification = bruh)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = bruh)]
	private void Dispose(bool disposing)
	{
		if (_disposedValue || _runnable is not IDisposable dr)
			return;

		dr.Dispose();
		_disposedValue = true;

		if (disposing)
			GC.SuppressFinalize(this);
	}

	~AsyncRunnableFinilizable() => this.Dispose(disposing: false);

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize", Justification = bruh)]
	public void Dispose() => this.Dispose(disposing: true);

	public Task<bool> IsRunning(CancellationToken token = default) => _runnable.IsRunning(token);
}
