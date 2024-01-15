namespace Name.Bayfaderix.Darxxemiyur.Abstract;

/// <summary>
/// Provides a wrapper for a type that should be identifyable.
/// </summary>
/// <typeparam name="TIdentifyable">A wrappable object.</typeparam>
public interface IIdentifiable<out TIdentifyable> : IMetaIdentity
{
	/// <summary>
	/// An optional IIdentifiable's channel to query it with whatever is needed. Full implementation(if
	/// any) is up to the user. Can be used to acquire additional objects, access dependency injection, and so on.
	/// </summary>
	IMessageCommunicable? QnA { get; }

	/// <summary>
	/// Identity of the IIdentifiable. Can be null.
	/// </summary>
	IIdentity? Identity
	{
		get;
	}

	/// <summary>
	/// The contained instance.
	/// </summary>
	TIdentifyable? Identifyable
	{
		get;
	}

	/// <summary>
	/// Compares two instances for equality.
	/// </summary>
	/// <param name="to"></param>
	/// <returns>True if they are equal according to the instance's type's implementation of the interface. False otherwise.</returns>
	bool Equals<TId>(IIdentifiable<TId> to);

	/// <summary>
	/// Compares two instances for equality.
	/// </summary>
	/// <param name="to"></param>
	/// <returns>True if they are equal according to the implementation of the interface. False otherwise.</returns>
	bool Equals(object to);
}
