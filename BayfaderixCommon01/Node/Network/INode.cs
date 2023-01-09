namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
	public interface INodeNetwork
	{
		/// <summary>
		/// Retreives starting instruction for the network.
		/// </summary>
		/// <returns></returns>
		NextNetworkInstruction GetStartingInstruction();

		/// <summary>
		/// Retreives starting instruction for the network.
		/// </summary>
		/// <param name="payload"></param>
		/// <returns></returns>
		NextNetworkInstruction GetStartingInstruction(object payload);

		/// <summary>
		/// Handler of step's result
		/// </summary>
		NodeResultHandler StepResultHandler {
			get;
		}

		/// <summary>
		/// Run this network.
		/// </summary>
		/// <returns></returns>
		Task<object> RunNetwork();
	}
}