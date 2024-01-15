using Name.Bayfaderix.Darxxemiyur.Abstract;
using Name.Bayfaderix.Darxxemiyur.General;

namespace Name.Bayfaderix.Darxxemiyur.Node.Network;

/// <summary>
/// General step info representing a unit of work in general node network.
/// </summary>
public interface IGeneralStepInfo : IMetaIdentity
{
	/// <summary>
	/// Optional implementation of message communicable of general step info.
	/// </summary>
	IMessageCommunicable? QnA => new StupidMessageCommunicable(false);

	/// <summary>
	/// Runs general step according to its implementation. Generally intended to run a single method.
	/// </summary>
	/// <param name="previous">Previous step info to take into consideration for this step info, if any.</param>
	/// <param name="token">Cancellation token</param>
	/// <returns>Next step info, if any.</returns>
	Task<IGeneralStepInfo?> Run(IGeneralStepInfo? previous = null, CancellationToken token = default);

	/// <summary>
	/// General network that this node thinks is its parent network, if any.
	/// </summary>
	IGeneralNodeNetwork? ParentNetwork {
		get;
	}
}
