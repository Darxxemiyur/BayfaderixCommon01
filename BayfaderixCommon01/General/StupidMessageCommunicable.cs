namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Stupid filler implementation of IMessageCommunicable that supports no communication. Has flag to
/// throw <see cref="NotImplementedException"/>
/// </summary>
public class StupidMessageCommunicable : IMessageCommunicable
{
	private readonly bool _flag;

	public StupidMessageCommunicable(bool throwOnAccess = true) => _flag = throwOnAccess;

	public CommunicableCapabilities Capabilities => CommunicableCapabilities.None;
	public Task<CommunicableCapabilities> CapabilitiesAsync => Task.FromResult(Capabilities);

	public TellResult TellInternal(TellMessage message) => _flag ? throw new NotImplementedException() : new TellResult(null);

	public Task<TellResult> TellInternalAsync(TellMessage message) => _flag ? throw new NotImplementedException() : Task.FromResult(new TellResult(null));

	public IEnumerable<TellResult> TellInternalProcedurally(TellMessage message)
	{
		yield return _flag ? throw new NotImplementedException() : new TellResult(null);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	//It is fine since it practically has no overhead.
	public async IAsyncEnumerable<TellResult> TellInternalProcedurallyAsync(TellMessage message)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		yield return _flag ? throw new NotImplementedException() : new TellResult(null);
	}
}
