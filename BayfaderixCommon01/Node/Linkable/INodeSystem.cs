namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public interface INodeSystem : Common.IAsyncRunnable
	{
		IEnumerable<INode> Network {
			get;
		}

		Task LinkSystem();
	}
}
