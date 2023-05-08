namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Provides a wrapper for a type that should be identifyable.
/// </summary>
/// <typeparam name="TIdentifyable">A wrappable object.</typeparam>
public interface IIdentifiable<out TIdentifyable> : IMetaIdentity
{
	/// <summary>
	/// Identity of the IIdentifiable. Can be null.
	/// </summary>
	IIdentity? Identity {
		get;
	}

	/// <summary>
	/// The contained instance.
	/// </summary>
	TIdentifyable? Identifyable {
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
