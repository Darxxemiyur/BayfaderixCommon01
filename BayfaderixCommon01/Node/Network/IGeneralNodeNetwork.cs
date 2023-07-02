using Name.Bayfaderix.Darxxemiyur.General;

namespace Name.Bayfaderix.Darxxemiyur.Node.Network;

/// <summary>
/// Represents a collection of general nodes, allowing to traverse them only how their implementation allows to.
/// </summary>
public interface IGeneralNodeNetwork : IMetaIdentity
{
	/// <summary>
	/// Optional implementation of message communicable of general node network.
	/// </summary>
	IMessageCommunicable? QnA => new StupidMessageCommunicable(false);

	/// <summary>
	/// Acquires general step info according to the payload.
	/// </summary>
	/// <param name="payload">Initial step info.</param>
	/// <returns>Respective general step info.</returns>
	Task<IGeneralStepInfo?> GetStartStepInfo(IGeneralStepInfo? payload);

	/// <summary>
	/// Acquires general step info according to the payload.
	/// </summary>
	/// <param name="payload">Initial step info.</param>
	/// <returns>Respective general step info.</returns>
	Task<IGeneralStepInfo?> GetStartStepInfo(object? payload = null);
}
