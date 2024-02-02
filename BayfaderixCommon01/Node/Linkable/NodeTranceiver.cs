namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public delegate IAsyncEnumerable<INodeContainer> Retriever();
public class NodeTranceiver : INodeTranceiver
{
	private INodeReceiver? _link;
	private readonly bool _configureAwait;
	private readonly Func<IAsyncEnumerable<INodeContainer>> _puller;
	public NodeTranceiver(Func<IAsyncEnumerable<INodeContainer>> puller, bool configureAwait = false) => (_puller, _configureAwait) = (puller, configureAwait);

	public async Task Link(INodeReceiver sink)
	{
		if (_link == sink)
			return;
		await this.UnLink();
		_link = sink;
		await sink.Link(this).ConfigureAwait(_configureAwait);
	}

	public async Task UnLink()
	{
		var link = _link;
		if (link == null)
			return;
		_link = null;
		await link.UnLink().ConfigureAwait(_configureAwait);
	}

	public Task Push(IAsyncEnumerable<INodeContainer> item) => _link?.Push(item) ?? Task.CompletedTask;

	public IAsyncEnumerable<INodeContainer> Retrieve() => (IAsyncEnumerable<INodeContainer>)_puller?.DynamicInvoke();
}
