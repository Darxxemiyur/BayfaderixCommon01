namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public class NodeTranceiver : INodeTranceiver
	{
		private readonly List<INodeLink> _outputLinks;

		public NodeTranceiver() => _outputLinks = new();

		public async Task Link(INodeReceiver sink)
		{
			var link = new ItemInstantTransferLink(this, sink);
			await sink.Link(link);
			await Link(link);
		}

		public Task Link(INodeLink link)
		{
			_outputLinks.Add(link);
			return Task.CompletedTask;
		}

		public async Task UnLink(INodeReceiver sink)
		{
			if (_outputLinks.Find(x => x.IsThisPair(this, sink)) is var link == default)
				return;
			await UnLink(link);
			await sink.UnLink(link);
		}

		public Task UnLink(INodeLink link) => Task.FromResult(_outputLinks.Remove(link));

		public Task Propogate(INodeContainer item) => Task.WhenAll(_outputLinks.Select(x => x.Propogate(item)));
	}
}