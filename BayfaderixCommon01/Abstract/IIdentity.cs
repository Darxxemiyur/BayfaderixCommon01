﻿namespace Name.Bayfaderix.Darxxemiyur.Abstract;

/// <summary>
/// Identity interface. Acts as an ambigous way to uniquely identify data.
/// Implementation is allowed to utilize any way of identification.
/// Hence should be used with care and only in places with convention.
/// </summary>
public interface IIdentity : IMetaIdentity
{
	/// <summary>
	/// An optional IIdentity's channel to query it with whatever is needed. Full implementation(if
	/// any) is up to the user. Can be used to acquire additional objects, access dependency injection, and so on.
	/// </summary>
	IMessageCommunicable? QnA { get; }

	/// <summary>
	/// Compares two instances for equality.
	/// </summary>
	/// <param name="to"></param>
	/// <returns>True if they are equal according to the implementation of the interface. False otherwise.</returns>
	bool Equals(IIdentity to);

	/// <summary>
	/// Whether this identity 'record' has id data to identify some other data.
	/// </summary>
	bool HasId
	{
		get;
	}
}