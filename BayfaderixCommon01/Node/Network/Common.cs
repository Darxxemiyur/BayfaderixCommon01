namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
	public delegate Task<bool> NodeResultHandler(StepInfo args, CancellationToken token = default);

	public static class NetworkCommon
	{
		public static Task<StepInfo?> RunNetwork(INodeNetwork net, object payload, CancellationToken token = default, bool configureAwait = false) => RunNetwork(net, net.StepResultHandler, payload, token, configureAwait);

		public static Task<StepInfo?> RunNetwork(INodeNetwork net, NodeResultHandler handler, object payload, CancellationToken token = default, bool configureAwait = false) => RunNetwork(net.GetStartingInstruction(payload), handler, token, configureAwait);

		public static Task<StepInfo?> RunNetwork(INodeNetwork net, CancellationToken token = default, bool configureAwait = false) => RunNetwork(net, net.StepResultHandler, token, configureAwait);

		public static Task<StepInfo?> RunNetwork(INodeNetwork net, NodeResultHandler handler, CancellationToken token = default, bool configureAwait = false) => RunNetwork(net.GetStartingInstruction(), handler, token, configureAwait);

		public static async Task<StepInfo?> RunNetwork(StepInfo? inst, NodeResultHandler handler, CancellationToken token = default, bool configureAwait = false)
		{
			while (inst?.NextStep != null && await handler(inst, token).ConfigureAwait(configureAwait))
				inst = await inst.NextStep(inst).ConfigureAwait(configureAwait);

			return inst;
		}
	}
}
