namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	public class NodeContainer<TItem> : INodeContainer<TItem>
	{
		private readonly TItem _containedItem;

		public ulong ContainerID {
			get;
		}

		public string ItemType => typeof(TItem).FullName;

		public object Custom {
			get;
		}

		public IEnumerable<INodeContainer> PreviousContainers {
			get;
		}

		public NodeContainer(ulong containerId, TItem tItem, object? custom = null)
		{
			ContainerID = containerId;
			_containedItem = tItem;
			PreviousContainers = new INodeContainer[] { this };
			Custom = custom;
		}

		private NodeContainer(ulong containerId, TItem tItem, IEnumerable<INodeContainer> previousContainers, object? custom = null)
		{
			ContainerID = containerId;
			_containedItem = tItem;
			PreviousContainers = previousContainers;
			Custom = custom;
		}

		public INodeContainer ReInvent(ulong newId)
		{
			return new NodeContainer<TItem>(newId, _containedItem, new[] { this }, Custom);
		}

		public override string ToString() => _containedItem.ToString();

		public DescriptioningData<TItem> GetContainedTypedData<InT>(DescriptioningInput<InT> payload)
		{
			return new(_containedItem);
		}

		public DescriptioningData<object> GetContainedData(DescriptioningInput<object> payload)
		{
			return new(_containedItem);
		}

		public INodeContainer<CItem> TryCast<CItem>() => this as INodeContainer<CItem>;
	}
}
