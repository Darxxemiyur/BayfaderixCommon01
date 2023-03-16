namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public interface INodeReceiver : ILinkable
	{
		Task Push(INodeContainer item);

		Task<INodeContainer> Retrieve();

		Task Link(INodeTranceiver source);

		Task UnLink(INodeTranceiver source);
	}
}
