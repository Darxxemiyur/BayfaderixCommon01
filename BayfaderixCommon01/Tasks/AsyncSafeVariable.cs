namespace Name.Bayfaderix.Darxxemiyur.Tasks;

/// <summary>
/// Variable that is safe to access and set in Async workflow
/// </summary>
/// <typeparam name="T"></typeparam>
public class AsyncSafeVariable<T>
{
	private T _value;
	private readonly AsyncLocker _sync;
	private readonly bool _configureAwait;

	public AsyncSafeVariable(bool configureAwait = false) : this(default, configureAwait)
	{
	}

	public AsyncSafeVariable(T value, bool configureAwait = false)
	{
		_configureAwait = configureAwait;
		_value = value;
		_sync = new();
	}

	public async Task SetValue(T value)
	{
		await using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
		_value = value;
	}

	public async Task SetValue(Func<T, Task<T>> value)
	{
		await using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
		_value = await value(_value).ConfigureAwait(_configureAwait);
	}

	public async Task<T> GetValue()
	{
		await using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
		var val = _value;

		return val;
	}

	public async Task<T> LocklyModValue(Func<T, Task<T>> value)
	{
		await using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
		return _value = await value(_value).ConfigureAwait(_configureAwait);
	}

	public static implicit operator T(AsyncSafeVariable<T> val) => val._value;

	public static implicit operator AsyncSafeVariable<T>(T val) => new(val);
}
