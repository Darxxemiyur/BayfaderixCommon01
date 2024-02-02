namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public interface INodeTranceiver
{
	Task Push(IAsyncEnumerable<INodeContainer> item);

	IAsyncEnumerable<INodeContainer> Retrieve();

	Task Link(INodeReceiver sink);

	Task UnLink();
}
