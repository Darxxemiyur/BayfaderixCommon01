namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
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
		StepInfo GetStartingInstruction(object? payload) => GetStartingInstruction();

		/// <summary>
		/// Handler of step's result
		/// </summary>
		NodeResultHandler StepResultHandler {
			get => (x, y) => Task.FromResult(!y.IsCancellationRequested);
		}

		/// <summary>
		/// Run this network.
		/// </summary>
		/// <returns></returns>
		Task<StepInfo?> RunNetwork() => NetworkCommon.RunNetwork(this, default, true);
	}
}