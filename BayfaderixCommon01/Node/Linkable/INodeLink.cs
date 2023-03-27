namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public interface INodeLink
{
	Task Propogate(INodeContainer item);

	Task Invalidate();

	bool IsThisPair(INodeTranceiver tr, INodeReceiver re);

	Task<INodeContainer> Retrieve();
}
