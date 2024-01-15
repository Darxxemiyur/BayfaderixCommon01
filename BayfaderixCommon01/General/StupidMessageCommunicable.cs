using Name.Bayfaderix.Darxxemiyur.Abstract;

namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Stupid filler implementation of IMessageCommunicable that supports no communication. Has flag to throw <see cref="NotImplementedException"/>
/// </summary>
public class StupidMessageCommunicable : IMessageCommunicable
{
	private readonly bool _flag;

	public StupidMessageCommunicable(bool throwOnAccess = true) => _flag = throwOnAccess;

	public CommunicableCapabilities Capabilities => CommunicableCapabilities.None;
	public Task<CommunicableCapabilities> CapabilitiesAsync => Task.FromResult(Capabilities);

	public ITellResult<object> TellInternal(ITellMessage<object> message) => _flag ? throw new NotImplementedException() : new TellResult<object>(null);

	Task<ITellResult<T>> IMessageCommunicable<object, object, object>.TellInternalAsync<T>(ITellMessage<object> message) => this.TellInternalAsync<T>(message);

	public Task<ITellResult<T>> TellInternalAsync<T>(ITellMessage<object> message) => _flag ? throw new NotImplementedException() : Task.FromResult<ITellResult<T>>(new TellResult<T>(null));

	public IEnumerable<ITellResult<object>> TellInternalProcedurally(ITellMessage<object> message)
	{
		yield return _flag ? throw new NotImplementedException() : new TellResult<object>(null);
	}

	public IEnumerable<ITellResult<object>> TellInternalProcedurally(IEnumerable<ITellMessage<object>> message)
	{
		yield return _flag ? throw new NotImplementedException() : new TellResult<object>(null);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

	//It is fine since it practically has no overhead.
	public async IAsyncEnumerable<ITellResult<object>> TellInternalProcedurallyAsync(ITellMessage<object> message)
	{
		yield return _flag ? throw new NotImplementedException() : new TellResult<object>(null);
	}

	public async IAsyncEnumerable<ITellResult<object>> TellInternalProcedurallyAsync(IAsyncEnumerable<ITellMessage<object>> message)
	{
		yield return _flag ? throw new NotImplementedException() : new TellResult<object>(null);
	}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
