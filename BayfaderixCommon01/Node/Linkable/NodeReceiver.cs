using Name.Bayfaderix.Darxxemiyur.Common;

namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public class NodeReceiver : INodeReceiver
	{
		private readonly List<INodeLink> _inputLinks;
		private readonly FIFOFBACollection<INodeContainer> _itemList;

		public NodeReceiver() => (_inputLinks, _itemList) = (new(), new());

		public async Task Link(INodeTranceiver source)
		{
			var link = new ItemInstantTransferLink(source, this);
			await source.Link(link);
			await Link(link);
		}

		public Task Link(INodeLink link)
		{
			_inputLinks.Add(link);
			return Task.CompletedTask;
		}

		public Task<INodeContainer> Retrieve() => _itemList.GetData();

		public Task Push(INodeContainer item) => _itemList.Handle(item);

		public async Task UnLink(INodeTranceiver source)
		{
			if (_inputLinks.Find(x => x.IsThisPair(source, this)) is var link == default)
				return;
			await UnLink(link);
			await source.UnLink(link);
		}

		public Task UnLink(INodeLink link) => Task.FromResult(_inputLinks.Remove(link));
	}
}