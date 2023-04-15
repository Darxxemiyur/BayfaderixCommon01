namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public class NodeTranceiver : INodeTranceiver
{
	private readonly List<INodeLink> _outputLinks;
	private readonly bool _configureAwait;

	public NodeTranceiver(bool configureAwait = false) => (_configureAwait, _outputLinks) = (configureAwait, new());

	public async Task Link(INodeReceiver sink)
	{
		var link = new ItemInstantTransferLink(this, sink);
		await sink.Link(link).ConfigureAwait(_configureAwait);
		await this.Link(link).ConfigureAwait(_configureAwait);
	}

	public Task Link(INodeLink link)
	{
		_outputLinks.Add(link);
		return Task.CompletedTask;
	}

	public async Task UnLink(INodeReceiver sink)
	{
		if (_outputLinks.Find(x => x.IsThisPair(this, sink)) is var link && link == default)
			return;
		await this.UnLink(link).ConfigureAwait(_configureAwait);
		await sink.UnLink(link).ConfigureAwait(_configureAwait);
	}

	public Task UnLink(INodeLink link) => Task.FromResult(_outputLinks.Remove(link));

	public Task Propogate(INodeContainer item) => Task.WhenAll(_outputLinks.Select(x => x.Propogate(item)));
}
