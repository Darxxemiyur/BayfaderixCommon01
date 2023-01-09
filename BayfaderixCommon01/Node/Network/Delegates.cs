namespace Name.Bayfaderix.Darxxemiyur.Node.Network
{
	public delegate Task<NextNetworkInstruction> Node<in Instruction>(Instruction args) where Instruction : NextNetworkInstruction;

	public delegate Task<bool> NodeResultHandler(NextNetworkInstruction args, CancellationToken token = default);
}