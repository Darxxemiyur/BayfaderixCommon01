using Name.Bayfaderix.Darxxemiyur.Common;

namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public class ItemQueuedTransferLink : INodeLink
	{
		private readonly FIFOFBACollection<INodeContainer> _itemList;
		private readonly INodeTranceiver _from;
		private readonly INodeReceiver _to;

		public ItemQueuedTransferLink(INodeTranceiver from, INodeReceiver to)
		 => (_from, _to, _itemList) = (from, to, new());

		public bool IsThisPair(INodeTranceiver tr, INodeReceiver re) => _from == tr && _to == re;

		public Task Propogate(INodeContainer item) => _itemList.Handle(item);

		public Task<INodeContainer> Retrieve() => _itemList.GetData();

		public Task Invalidate() => throw new NotImplementedException();
	}
}