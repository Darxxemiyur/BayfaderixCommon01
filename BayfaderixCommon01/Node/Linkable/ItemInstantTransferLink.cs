namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public class ItemInstantTransferLink : INodeLink
{
	private readonly INodeTranceiver _from;
	private readonly INodeReceiver _to;

	public ItemInstantTransferLink(INodeTranceiver from, INodeReceiver to)
	 => (_from, _to) = (from, to);

	public Task Propogate(INodeContainer item) => _to.Push(item);

	public bool IsThisPair(INodeTranceiver tr, INodeReceiver re) => _from == tr && _to == re;

	public Task<INodeContainer> Retrieve() => throw new NotImplementedException();

	public Task Invalidate() => throw new NotImplementedException();
}
