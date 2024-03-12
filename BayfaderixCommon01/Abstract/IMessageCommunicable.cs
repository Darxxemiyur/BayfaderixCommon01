namespace Name.Bayfaderix.Darxxemiyur.Abstract;

/// <summary>
/// Presents an interface to allow generic communication with an object.
/// </summary>
public interface IMessageCommunicable<out TResult, in TMessage, TResultParent> where TResult : TResultParent
{
	/// <summary>
	/// Acquire instance communication capabilities.
	/// </summary>
	CommunicableCapabilities Capabilities
	{
		get;
	}

	/// <summary>
	/// Acquire instance communication capabilities asynchroniously.
	/// </summary>
	Task<CommunicableCapabilities> CapabilitiesAsync
	{
		get;
	}

	/// <summary>
	/// Communicate instance an object.
	/// </summary>
	/// <returns>The tell result.</returns>
	ITellResult<TResult> TellInternal(ITellMessage<TMessage> message);

	/// <summary>
	/// Communicate instance an object asynchroniously.
	/// </summary>
	/// <returns>The awaitable tell result.</returns>
	Task<ITellResult<T>> TellInternalAsync<T>(ITellMessage<TMessage> message) where T : TResultParent;

	/// <summary>
	/// Communicate instance an object.
	/// </summary>
	/// <returns>The procedural communication channel that is up to the session channel instance to implement.</returns>
	IEnumerable<ITellResult<TResult>> TellInternalProcedurally(ITellMessage<TMessage> message);

	/// <summary>
	/// Communicate instance an object.
	/// </summary>
	/// <returns>The procedural communication channel that is up to the session channel instance to implement.</returns>
	IEnumerable<ITellResult<TResult>> TellInternalProcedurally(IEnumerable<ITellMessage<TMessage>> message);

	/// <summary>
	/// Communicate instance an object asynchroniously.
	/// </summary>
	/// <returns>The procedural asynchronous communication channel that is up to the session channel instance to implement.</returns>
	IAsyncEnumerable<ITellResult<TResult>> TellInternalProcedurallyAsync(ITellMessage<TMessage> message);

	/// <summary>
	/// Communicate instance an object asynchroniously.
	/// </summary>
	/// <returns>The procedural asynchronous communication channel that is up to the session channel instance to implement.</returns>
	IAsyncEnumerable<ITellResult<TResult>> TellInternalProcedurallyAsync(IAsyncEnumerable<ITellMessage<TMessage>> message);
}

/// <summary>
/// Default assisted IMessageCommunicable implementation.
/// </summary>
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
//............ Because you cannot have a constraint to derive from the System.Object in generics?
public interface IMessageCommunicable<TResult> : IMessageCommunicable<TResult, object, object>
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
{ }

/// <summary>
/// Default assisted IMessageCommunicable implementation.
/// </summary>
public interface IMessageCommunicable : IMessageCommunicable<object>
{ }
