using Name.Bayfaderix.Darxxemiyur.Collections;

namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public delegate Task NodeReceiverDelegate(IAsyncEnumerable<INodeContainer> items);

public class NodeReceiver : INodeReceiver
{
	private INodeTranceiver? _link;
	private readonly NodeReceiverDelegate _pusher;
	private readonly bool _configureAwait;

	public NodeReceiver(NodeReceiverDelegate receiver, bool configureAwait = false) => (_pusher, _configureAwait) = (receiver, configureAwait);

	public async Task Link(INodeTranceiver source)
	{
		if (_link == source)
			return;
		await this.UnLink();
		_link = source;
		await source.Link(this).ConfigureAwait(_configureAwait);
	}

	public async Task UnLink()
	{
		var link = _link;
		if (link == null)
			return;
		_link = null;
		await link.UnLink().ConfigureAwait(_configureAwait);
	}
	//Yes this is intentional.
	public IAsyncEnumerable<INodeContainer> Retrieve() => _link.Retrieve();

	public Task Push(IAsyncEnumerable<INodeContainer> item) => _pusher?.Invoke(item) ?? Task.CompletedTask;

}
