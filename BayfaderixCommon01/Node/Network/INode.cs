namespace Name.Bayfaderix.Darxxemiyur.Node.Network;

/// <summary>
/// An asynchronous node network. Provides a way to build a network of nodes that do certain tasks.
/// Building UI with it is one of many ways to use it.
/// </summary>
public interface INodeNetwork
{
	/// <summary>
	/// Retreives starting instruction for the network.
	/// </summary>
	/// <returns></returns>
	StepInfo GetStartingInstruction();

	/// <summary>
	/// Retreives starting instruction for the network.
	/// </summary>
	/// <param name="payload"></param>
	/// <returns></returns>
	StepInfo GetStartingInstruction(object? payload) => this.GetStartingInstruction();

	/// <summary>
	/// Handler of step's result
	/// </summary>
	NodeResultHandler StepResultHandler => (x, y) => Task.FromResult(!y.IsCancellationRequested);

	/// <summary>
	/// Run this network.
	/// </summary>
	/// <returns></returns>
	Task<StepInfo?> RunNetwork() => NetworkCommon.RunNetwork(this, default, true);
}
