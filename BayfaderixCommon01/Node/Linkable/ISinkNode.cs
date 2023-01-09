namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	/// <summary>
	/// Pipe that can act as a sink.
	/// </summary>
	public interface ISinkNode : INode
	{
		INodeReceiver ItemReceiver {
			get;
		}
	}
}