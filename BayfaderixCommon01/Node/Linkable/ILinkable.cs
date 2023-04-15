namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable;

public interface ILinkable
{
	Task Link(INodeLink link);

	Task UnLink(INodeLink link);
}
