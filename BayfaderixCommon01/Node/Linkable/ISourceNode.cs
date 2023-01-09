namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	/// <summary>
	/// Pipe that can act as a source
	/// </summary>
	public interface ISourceNode : INode
	{
		INodeTranceiver ItemTranceiver {
			get;
		}
	}
}