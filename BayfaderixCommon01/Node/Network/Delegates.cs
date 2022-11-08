namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
	public delegate Task<NextNetworkInstruction> Node(NetworkInstructionArgument args);

	public delegate Task<bool> NodeResultHandler(NextNetworkInstruction args, CancellationToken token = default);
}