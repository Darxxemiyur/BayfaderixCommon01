namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	/// <summary>
	/// Pipe that can act both as a sink and a source
	/// </summary>
	public interface IBiNode : ISinkNode, ISourceNode, INode
	{
	}
}
