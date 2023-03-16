namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public abstract class BaseNodeSystem : Common.IAsyncRunnable, INodeSystem
	{
		protected BaseNodeSystem()
		{
			_network = new();
		}

		public IEnumerable<INode> Network => _network;
		protected readonly List<INode> _network;

		public abstract Task LinkSystem();

		public Task RunRunnable(CancellationToken token = default) => Task.WhenAll(Network.Select(x => x.RunRunnable()));
	}
}
