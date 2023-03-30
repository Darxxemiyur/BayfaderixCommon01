namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Presents an interface to allow generic communication with an object.
/// </summary>
public interface IMessageCommunicable
{
	/// <summary>
	/// Acquire instance communication capabilities.
	/// </summary>
	CommunicableCapabilities Capabilities {
		get;
	}

	/// <summary>
	/// Acquire instance communication capabilities asynchroniously.
	/// </summary>
	Task<CommunicableCapabilities> CapabilitiesAsync {
		get;
	}

	/// <summary>
	/// Communicate instance an object.
	/// </summary>
	/// <returns>The tell result.</returns>
	TellResult TellInternal(TellMessage message);

	/// <summary>
	/// Communicate instance an object.
	/// </summary>
	/// <returns>
	/// The procedural communication channel that is up to the session channel instance to implement.
	/// </returns>
	IEnumerable<TellResult> TellInternalProcedurally(TellMessage message);

	/// <summary>
	/// Communicate instance an object asynchroniously.
	/// </summary>
	/// <returns>The awaitable tell result.</returns>
	Task<TellResult> TellInternalAsync(TellMessage message);

	/// <summary>
	/// Communicate instance an object asynchroniously.
	/// </summary>
	/// <returns>
	/// The procedural asynchronous communication channel that is up to the session channel instance
	/// to implement.
	/// </returns>
	IAsyncEnumerable<TellResult> TellInternalProcedurallyAsync(TellMessage message);

	/// <summary>
	/// Communicate instance an object.
	/// </summary>
	/// <returns>
	/// The procedural communication channel that is up to the session channel instance to implement.
	/// </returns>
	IEnumerable<TellResult> TellInternalProcedurally(IEnumerable<TellMessage> message);

	/// <summary>
	/// Communicate instance an object asynchroniously.
	/// </summary>
	/// <returns>
	/// The procedural asynchronous communication channel that is up to the session channel instance
	/// to implement.
	/// </returns>
	IAsyncEnumerable<TellResult> TellInternalProcedurallyAsync(IAsyncEnumerable<TellMessage> message);
}
