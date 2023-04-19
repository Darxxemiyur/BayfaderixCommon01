namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Communication capabilities of IMessageCommunicable instance.
/// </summary>
[Flags]
public enum CommunicableCapabilities
{
	/// <summary>
	/// Ask <see cref="IMessageCommunicable.CapabilitiesAsync"/> for supported comm ways.
	/// </summary>
	AskAsyncCapabilities = 1 << 8,

	/// <summary>
	/// Communicable supports no interactions.
	/// </summary>
	None = 0,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternal(TellMessage)"/>.
	/// </summary>
	TellInternal = 1 << 0,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalProcedurally(TellMessage)"/>.
	/// </summary>
	TellInternalProcedurally = 1 << 1,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalAsync(TellMessage)"/>.
	/// </summary>
	TellInternalAsync = 1 << 2,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalProcedurallyAsync(TellMessage)"/>.
	/// </summary>
	TellInternalProcedurallyAsync = 1 << 3,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.Capabilities"/>.
	/// </summary>
	Capabilities = 1 << 4,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.CapabilitiesAsync"/>.
	/// </summary>
	CapabilitiesAsync = 1 << 5,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalProcedurally(IEnumerable{TellMessage})"/>.
	/// </summary>
	TellInternalProcedurallyFromProcedural = 1 << 6,

	/// <summary>
	/// Supports and implements <see cref="IMessageCommunicable.TellInternalProcedurallyAsync(IAsyncEnumerable{TellMessage})"/>.
	/// </summary>
	TellInternalProcedurallyFromProceduralAsync = 1 << 7,

	/// <summary>
	/// Supports and implements everything these do: <see cref="TellInternal"/>, <see cref="TellInternalProcedurally"/>, <see cref="Capabilities"/>, <see cref="TellInternalProcedurallyFromProcedural"/>.
	/// </summary>
	AllSync = TellInternal | TellInternalProcedurally | Capabilities | TellInternalProcedurallyFromProcedural,

	/// <summary>
	/// Supports and implements everything these do: <see cref="TellInternalAsync"/>, <see cref="TellInternalProcedurallyAsync"/>, <see cref="CapabilitiesAsync"/>, <see cref="TellInternalProcedurallyFromProceduralAsync"/>.
	/// </summary>
	AllAsync = TellInternalAsync | TellInternalProcedurallyAsync | CapabilitiesAsync | TellInternalProcedurallyFromProceduralAsync,

	/// <summary>
	/// Supports and implements everything these do: <see cref="AllSync"/> and <see cref="AllAsync"/>.
	/// </summary>
	All = AllSync | AllAsync,
}
