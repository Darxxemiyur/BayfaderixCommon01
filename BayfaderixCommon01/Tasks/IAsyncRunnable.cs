namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Async runnable task. Does not provide any way to determine whether it was already ran or not, you must check it on your own!
/// </summary>
public interface IAsyncRunnable
{
	/// <summary>
	/// Starts the runnable(a long-term task)
	/// </summary>
	/// <returns></returns>
	Task RunRunnable(CancellationToken token = default);
}
