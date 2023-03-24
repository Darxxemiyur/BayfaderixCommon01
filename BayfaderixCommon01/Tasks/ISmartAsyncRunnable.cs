using Name.Bayfaderix.Darxxemiyur.Common;

namespace Name.Bayfaderix.Darxxemiyur.General
{
	/// <summary>
	/// Smart async runnable that can be started and stopped.
	/// </summary>
	public interface ISmartAsyncRunnable<T>
	{
		MyTaskSource<T> ExposedObject {
			get;
		}

		/// <summary>
		/// Starts the runnable(a long-term task)
		/// </summary>
		/// <param name="token">The cancellation token to stop the runnable running.</param>
		/// <returns></returns>
		Task RunRunnable(CancellationToken token = default);

		/// <summary>
		/// Reports if the runnable is running.
		/// </summary>
		/// <param name="token">The cancellation token to stop the status fetching.</param>
		/// <returns></returns>
		Task<bool> IsRunning(CancellationToken token = default);

		/// <summary>
		/// Stops async runnable.
		/// </summary>
		/// <param name="token">The cancellation token to stop the runnable stopping.</param>
		/// <returns></returns>
		Task StopRunnable(CancellationToken token = default);
	}

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public interface ISmartAsyncRunnable : ISmartAsyncRunnable<object>
	{
	}
}
