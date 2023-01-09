namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	/// <summary>
	/// Container for nodes to utilize only single real type that has other types stacked in it.
	/// Like a converyor belt that carries one box per time unit that can has anything in it.
	/// </summary>
	public interface INodeContainer
	{
		ulong ContainerID {
			get;
		}

		string ItemType {
			get;
		}

		object Custom {
			get;
		}

		DescriptioningData<object> GetContainedData(DescriptioningInput<object> payload);

		IEnumerable<INodeContainer> PreviousContainers {
			get;
		}

		INodeContainer ReInvent(ulong newId);

		INodeContainer<CItem> TryCast<CItem>();
	}

	public interface INodeContainer<TItem> : INodeContainer
	{
		DescriptioningData<TItem> GetContainedTypedData<InT>(DescriptioningInput<InT> payload);
	}
}