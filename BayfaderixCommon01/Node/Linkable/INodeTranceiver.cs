namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public interface INodeTranceiver : ILinkable
	{
		Task Propogate(INodeContainer item);

		Task Link(INodeReceiver sink);

		Task UnLink(INodeReceiver sink);
	}
}