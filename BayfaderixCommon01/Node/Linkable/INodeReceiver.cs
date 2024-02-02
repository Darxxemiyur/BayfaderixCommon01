namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public interface INodeReceiver
{
	Task Push(IAsyncEnumerable<INodeContainer> item);

	IAsyncEnumerable<INodeContainer> Retrieve();

	Task Link(INodeTranceiver source);

	Task UnLink();
}
