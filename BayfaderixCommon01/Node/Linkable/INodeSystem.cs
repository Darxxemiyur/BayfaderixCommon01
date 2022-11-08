namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public interface INodeSystem : Common.IAsyncRunnable
	{
		IEnumerable<INode> Network {
			get;
		}

		Task LinkSystem();
	}

	public abstract class BaseNodeSystem : Common.IAsyncRunnable, INodeSystem
	{
		protected BaseNodeSystem()
		{
			_network = new();
		}

		public IEnumerable<INode> Network => _network;
		protected readonly List<INode> _network;

		public abstract Task LinkSystem();

		public Task RunRunnable() => Task.WhenAll(Network.Select(x => x.RunRunnable()));
	}
}