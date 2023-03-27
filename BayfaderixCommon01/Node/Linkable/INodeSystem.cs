namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public interface INodeSystem : Tasks.IAsyncRunnable
{
	IEnumerable<INode> Network {
		get;
	}

	Task LinkSystem();
}
