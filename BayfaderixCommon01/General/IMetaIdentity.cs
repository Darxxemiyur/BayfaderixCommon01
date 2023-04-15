namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Meta(data) identity to identify run-time types.
/// </summary>
public interface IMetaIdentity
{
	/// <summary>
	/// Type of the instance.
	/// </summary>
	Type Type => this.GetType(); //Default "the fuckin forget" implementation.
}
