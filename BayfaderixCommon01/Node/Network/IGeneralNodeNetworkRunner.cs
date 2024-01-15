using Name.Bayfaderix.Darxxemiyur.Abstract;
using Name.Bayfaderix.Darxxemiyur.General;

namespace Name.Bayfaderix.Darxxemiyur.Node.Network;

/// <summary>
/// General network(s) runner. The loop of traversing one or multiple general node networks.
/// </summary>
public interface IGeneralNodeNetworkRunner : IMetaIdentity
{
	/// <summary>
	/// Optional implementation of message communicable of general node network runner.
	/// </summary>
	IMessageCommunicable? QnA => new StupidMessageCommunicable(false);

	/// <summary>
	/// Run general node network.
	/// </summary>
	/// <param name="network">General node netowrk to run.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Resulting general step info, if any.</returns>
	Task<IGeneralStepInfo?> RunNetwork(IGeneralNodeNetwork network, CancellationToken token = default);

	/// <summary>
	/// Run network according to general step info.
	/// </summary>
	/// <param name="stepInfo">General step info to run the network of from.</param>
	/// <param name="token">Cancellation token.</param>
	/// <returns>Resulting general step info, if any.</returns>
	Task<IGeneralStepInfo?> RunNetwork(IGeneralStepInfo stepInfo, CancellationToken token = default);
}
