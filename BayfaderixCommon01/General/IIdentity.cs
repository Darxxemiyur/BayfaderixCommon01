namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Identity interface. Acts as an ambigous way to uniquely identify data.
/// </summary>
public interface IIdentity : IMetaIdentity
{
	/// <summary>
	/// An optional IIdentity's channel to query it with whatever is needed. Full implementation(if
	/// any) is up to the user. Can be used to acquire additional objects, access dependency injection, and so on.
	/// </summary>
	IMessageCommunicable? QnA {
		get;
	}

	/// <summary>
	/// Compares two instances for equality.
	/// </summary>
	/// <param name="to"></param>
	/// <returns>True if they are equal according to the implementation of the interface. False otherwise.</returns>
	bool Equals(IIdentity to);
}
