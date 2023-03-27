namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Communication capabilities of IMessageCommunicable instance.
/// </summary>
[Flags]
public enum CommunicableCapabilities
{
	/// <summary>
	/// Communicable supports no interactions.
	/// </summary>
	None = 0,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternal(TellMessage)">TellInternal</see>.
	/// </summary>
	TellInternal = 1 << 0,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalProcedurally(TellMessage)(TellMessage)">TellInternalProcedurally</see>.
	/// </summary>
	TellInternalProcedurally = 1 << 1,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalAsync(TellMessage)(TellMessage)">TellInternalAsync</see>.
	/// </summary>
	TellInternalAsync = 1 << 2,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalProcedurallyAsync(TellMessage)(TellMessage)">TellInternalProcedurallyAsync</see>.
	/// </summary>
	TellInternalProcedurallyAsync = 1 << 3,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.Capabilities">Capabilities</see>.
	/// </summary>
	Capabilities = 1 << 4,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.CapabilitiesAsync">CapabilitiesAsync</see>.
	/// </summary>
	CapabilitiesAsync = 1 << 5,

	/// <summary>
	/// Supports and implements everything these do: <see cref="TellInternal"/>, <see
	/// cref="TellInternalProcedurally"/>, <see cref="Capabilities"/>.
	/// </summary>
	AllSync = TellInternal | TellInternalProcedurally | Capabilities,

	/// <summary>
	/// Supports and implements everything these do: <see cref="TellInternalAsync"/>, <see
	/// cref="TellInternalProcedurallyAsync"/>, <see cref="CapabilitiesAsync"/>.
	/// </summary>
	AllAsync = TellInternalAsync | TellInternalProcedurallyAsync | CapabilitiesAsync,

	/// <summary>
	/// Supports and implements everything these do: <see cref="AllSync"/> and <see cref="AllAsync"/>.
	/// </summary>
	All = AllSync | AllAsync,
}
