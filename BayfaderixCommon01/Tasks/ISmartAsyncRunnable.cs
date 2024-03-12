using System.Threading.Tasks;

namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Smart async runnable that can be started and stopped with an exposed object.
/// </summary>
public interface ISmartAsyncRunnable<T> : ISmartAsyncRunnable
{
	/// <summary>
	/// A retrievable object exposed by the instance.
	/// </summary>
	Task<T> ExposedObject
	{
		get;
	}
}

/// <summary>
/// Smart async runnable that can be started and stopped.
/// </summary>
public interface ISmartAsyncRunnable
{
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
	/// <param name="token">The cancellation token to stop the smart runnable.</param>
	/// <returns></returns>
	Task StopRunnable(CancellationToken token = default);
}
