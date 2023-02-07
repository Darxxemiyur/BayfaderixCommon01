using Name.Bayfaderix.Darxxemiyur.Common.Extensions;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public sealed class MyTaskSource : IDisposable
	{
		private readonly MyTaskSource<bool> _facade;
		private bool disposedValue;

		public MyTaskSource(CancellationToken token = default, bool throwOnException = true) => _facade = new(token, throwOnException);

		public Task MyTask => _facade.MyTask;

		public Task<bool> TrySetResultAsync() => _facade.TrySetResultAsync(false);

		public Task<bool> TrySetCanceledAsync() => _facade.TrySetCanceledAsync();

		public Task<bool> TrySetExceptionAsync(Exception exception) => _facade.TrySetExceptionAsync(exception);

		public async Task MimicResult(Task mimic)
		{
			try
			{
				await mimic;
				await TrySetResultAsync();
			}
			catch (TaskCanceledException)
			{
				await TrySetCanceledAsync();
			}
			catch (Exception e)
			{
				await TrySetExceptionAsync(e);
			}
		}

		public async Task FollowResult(Task follow)
		{
			await MimicResult(follow);
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

		~MyTaskSource() => Dispose(disposing: false);

		public void Dispose()
		{
			Dispose(disposing: true);
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

		private Task<T> _innerTask;
		private bool disposedValue;

		public Task<T> MyTask => InSecure();

		public static implicit operator Task<T>(MyTaskSource<T> task) => task.MyTask;

		private async Task<T> InSecure()
		{
			await using (var _ = await _lockb.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
				_innerTask ??= InTask();

			return await _innerTask.ConfigureAwait(_configureAwait);
		}

		private async Task<T> InTask()
		{
			if (!_source.Task.IsCompleted)
				await Task.WhenAny(_source.Task, Task.Delay(-1, _inner)).ConfigureAwait(_configureAwait);
			//await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			await TrySetCanceledAsync().ConfigureAwait(_configureAwait);

			var result = await _source.Task.ConfigureAwait(_configureAwait);

			//If it's result, return.
			if (result.Item1)
				return (T)result.Item2;

			//Do a very very bad thing if we don't want exceptions
			if (!_throwOnException)
				return default;

			//Else throw exception.
			throw new MyTaskSourceException((Exception)result.Item2);
		}

		public async Task MimicResult(Task<T> mimic)
		{
			try
			{
				await TrySetResultAsync(await mimic);
			}
			catch (TaskCanceledException)
			{
				await TrySetCanceledAsync();
			}
			catch (Exception e)
			{
				await TrySetExceptionAsync(e);
			}
		}

		public async Task<T> FollowResult(Task<T> follow)
		{
			await MimicResult(follow);
			return await MyTask;
		}

		public bool TrySetResult(T result)
		{
			using var _ = _lock.BlockLock();

			return !_inner.IsCancellationRequested && _source.TrySetResult((true, result));
		}

		public bool TrySetException(Exception result)
		{
			using var _ = _lock.BlockLock();

			return !_inner.IsCancellationRequested && _source.TrySetResult((false, _throwOnException ? result : null));
		}

		public bool TrySetCanceled()
		{
			using var _ = _lock.BlockLock();

			if (!_inner.IsCancellationRequested)
				_cancel.Cancel();

			return _inner.IsCancellationRequested && _source.TrySetResult((false, _throwOnException ? new TaskCanceledException(null, null, _inner) : null));
		}

		/// <summary>
		/// Tries to set result. True if success, False if failure. Will fail if was cancelled.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public async Task<bool> TrySetResultAsync(T result)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);

			return !_inner.IsCancellationRequested && await MyTaskExtensions.RunOnScheduler(() => _source.TrySetResult((true, result)));
		}

		public async Task<bool> TrySetExceptionAsync(Exception result)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);

			return !_inner.IsCancellationRequested && await MyTaskExtensions.RunOnScheduler(() => _source.TrySetResult((false, _throwOnException ? result : null)));
		}

		public async Task<bool> TrySetCanceledAsync()
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			if (!_inner.IsCancellationRequested)
				_cancel.Cancel();

			return _inner.IsCancellationRequested && await MyTaskExtensions.RunOnScheduler(() => _source.TrySetResult((false, _throwOnException ? new TaskCanceledException(null, null, _inner) : null)));
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
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~MyTaskSource() => Dispose(disposing: false);
	}
}