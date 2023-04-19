using Name.Bayfaderix.Darxxemiyur.Extensions;

namespace Name.Bayfaderix.Darxxemiyur.Tasks;

public sealed class MyTaskSource : IDisposable
{
	private readonly MyTaskSource<bool> _facade;
	private bool disposedValue;

	public MyTaskSource(CancellationToken token = default, bool throwOnException = true, bool configureAwait = false) => _facade = new(token, throwOnException, configureAwait);

	public Task MyTask => _facade.MyTask;

	public Task<bool> TrySetResultAsync() => _facade.TrySetResultAsync(false);

	public Task<bool> TrySetCanceledAsync() => _facade.TrySetCanceledAsync();

	public Task<bool> TrySetExceptionAsync(Exception exception) => _facade.TrySetExceptionAsync(exception);

	public async Task MimicResult(Task mimic)
	{
		try
		{
			await mimic;
			await this.TrySetResultAsync();
		}
		catch (TaskCanceledException) when (mimic.IsCanceled)
		{
			await this.TrySetCanceledAsync();
		}
		catch (Exception e)
		{
			await this.TrySetExceptionAsync(e);
		}
	}

	public async Task FollowResult(Task follow)
	{
		await this.MimicResult(follow);
		await MyTask;
	}

	public bool TrySetResult() => _facade.TrySetResult(false);

	public bool TrySetCanceled() => _facade.TrySetCanceled();

	public bool TrySetException(Exception exception) => _facade.TrySetException(exception);

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				_facade.Dispose();
			}

			disposedValue = true;
		}
	}

	~MyTaskSource() => this.Dispose(disposing: false);

	public void Dispose()
	{
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}

/// <summary>
/// Task source wrapper.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MyTaskSource<T> : IDisposable
{
	private readonly TaskCompletionSource<(bool, object)> _source;
	private readonly AsyncLocker _lock;
	private readonly AsyncLocker _lockb;
	private readonly CancellationTokenSource _cancel;
	private readonly CancellationTokenSource _icancel;
	private readonly CancellationToken _inner;
	private readonly bool _throwOnException;
	private readonly bool _configureAwait;

	public MyTaskSource(CancellationToken token = default, bool throwOnException = true, bool configureAwait = false)
	{
		_lock = new();
		_lockb = new();
		_source = new();
		_throwOnException = throwOnException;
		_configureAwait = configureAwait;
		_cancel = new CancellationTokenSource();
		_icancel = CancellationTokenSource.CreateLinkedTokenSource(token, _cancel.Token);
		_inner = _icancel.Token;
	}

	private Task<T>? _innerTask;
	private bool disposedValue;

	public Task<T> MyTask => this.InSecure();

	public static implicit operator Task<T>(MyTaskSource<T> task) => task.MyTask;

	private async Task<T> InSecure()
	{
		await using (var _ = await _lockb.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
			_innerTask ??= this.InTask();

		return await _innerTask.ConfigureAwait(_configureAwait);
	}

	private async Task<T> InTask()
	{
		if (!_source.Task.IsCompleted)
			await Task.WhenAny(_source.Task, Task.Delay(-1, _inner)).ConfigureAwait(_configureAwait);
		await this.TrySetCanceledAsync().ConfigureAwait(_configureAwait);

		var result = await _source.Task.ConfigureAwait(_configureAwait);

		//If it's result, return.
		if (result.Item1)
			return (T)result.Item2;

#pragma warning disable CS8603 // Possible null reference return.
		//Do a very very bad thing if we don't want exceptions
		if (!_throwOnException)
			//That's a new todo.
			//TODO: Either get rid of _throwOnException in favor of always throwing if something happens, or change signature.
			return default;
#pragma warning restore CS8603 // Possible null reference return.

		//Else throw exception.
		throw new MyTaskSourceException((Exception)result.Item2);
	}

	public async Task MimicResult(Task<T> mimic)
	{
		try
		{
			await this.TrySetResultAsync(await mimic);
		}
		catch (TaskCanceledException) when (mimic.IsCanceled)
		{
			await this.TrySetCanceledAsync();
		}
		catch (Exception e)
		{
			await this.TrySetExceptionAsync(e);
		}
	}

	public async Task<T> FollowResult(Task<T> follow)
	{
		await this.MimicResult(follow);
		return await MyTask;
	}

	public bool TrySetResult(T result)
	{
		using var __ = _lock.ScopeLock();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
		//The result is not null.
		return !_inner.IsCancellationRequested && _source.TrySetResult((true, result));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
	}

	public bool TrySetException(Exception result)
	{
		using var __ = _lock.ScopeLock();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
		//The right slot of tuple is not null only if _throwOnException is true.
		return !_inner.IsCancellationRequested && _source.TrySetResult((false, _throwOnException ? result : null));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
	}

	public bool TrySetCanceled()
	{
		using var __ = _lock.ScopeLock();

		if (!_inner.IsCancellationRequested)
			_cancel.Cancel();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
		//The right slot of tuple is not null only if _throwOnException is true.
		return _inner.IsCancellationRequested && _source.TrySetResult((false, _throwOnException ? new TaskCanceledException(null, null, _inner) : null));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
	}

	/// <summary>
	/// Tries to set result. True if success, False if failure. Will fail if was cancelled.
	/// </summary>
	/// <param name="result"></param>
	/// <returns></returns>
	public async Task<bool> TrySetResultAsync(T result)
	{
		await using var __ = await _lock.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
		//The result is not null.
		return !_inner.IsCancellationRequested && await MyTaskExtensions.RunOnScheduler(() => _source.TrySetResult((true, result)));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
	}

	public async Task<bool> TrySetExceptionAsync(Exception result)
	{
		await using var __ = await _lock.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
		//The right slot of tuple is not null only if _throwOnException is true.
		return !_inner.IsCancellationRequested && await MyTaskExtensions.RunOnScheduler(() => _source.TrySetResult((false, _throwOnException ? result : null)));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
	}

	public async Task<bool> TrySetCanceledAsync()
	{
		await using var __ = await _lock.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
		if (!_inner.IsCancellationRequested)
			_cancel.Cancel();

#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
		//The right slot of tuple is not null only if _throwOnException is true.
		return _inner.IsCancellationRequested && await MyTaskExtensions.RunOnScheduler(() => _source.TrySetResult((false, _throwOnException ? new TaskCanceledException(null, null, _inner) : null)));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
			return;

		if (disposing)
		{
			_lock.Dispose();
			_cancel.Dispose();
			_icancel.Dispose();
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		this.Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~MyTaskSource() => this.Dispose(disposing: false);
}
