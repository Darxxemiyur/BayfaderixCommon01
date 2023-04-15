using Name.Bayfaderix.Darxxemiyur.Collections;

namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public class NodeReceiver : INodeReceiver
{
	private readonly List<INodeLink> _inputLinks;
	private readonly FIFOFBACollection<INodeContainer> _itemList;
	private readonly bool _configureAwait;

	public NodeReceiver(bool configureAwait = false) => (_inputLinks, _itemList, _configureAwait) = (new(), new(), configureAwait);

	public async Task Link(INodeTranceiver source)
	{
		var link = new ItemInstantTransferLink(source, this);
		await source.Link(link).ConfigureAwait(_configureAwait);
		await this.Link(link).ConfigureAwait(_configureAwait);
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
		if (_inputLinks.Find(x => x.IsThisPair(source, this)) is var link && link == default)
			return;
		await this.UnLink(link).ConfigureAwait(_configureAwait);
		await source.UnLink(link).ConfigureAwait(_configureAwait);
	}

	public Task UnLink(INodeLink link) => Task.FromResult(_inputLinks.Remove(link));
}
