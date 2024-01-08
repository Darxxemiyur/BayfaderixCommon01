namespace Name.Bayfaderix.Darxxemiyur.General;

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
public interface IMessageCommunicable : IMessageCommunicable<object, object, object>
{ }
