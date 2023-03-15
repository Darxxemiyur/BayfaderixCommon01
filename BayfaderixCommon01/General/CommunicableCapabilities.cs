namespace Name.Bayfaderix.Darxxemiyur.General
{
	/// <summary>
	/// Communication capabilities of IMessageCommunicable instance.
	/// </summary>
	[Flags]
	public enum CommunicableCapabilities
	{
		None = 0,
		TellInternal = 1 << 0,
		TellInternalProcedurally = 1 << 1,
		TellInternalAsync = 1 << 2,
		TellInternalProcedurallyAsync = 1 << 3,
		Capabilities = 1 << 4,
		CapabilitiesAsync = 1 << 5,
	}
}