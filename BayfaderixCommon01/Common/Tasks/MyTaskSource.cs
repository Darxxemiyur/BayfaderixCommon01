namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class MyTaskSource : IDisposable
	{
		private readonly MyTaskSource<bool> _facade;
		private bool disposedValue;

		public MyTaskSource(CancellationToken token = default, bool throwOnException = true) => _facade = new(token, throwOnException);

		public Task MyTask => _facade.MyTask;

		public Task<bool> TrySetResultAsync() => _facade.TrySetResultAsync(false);

		public Task<bool> TrySetCanceledAsync() => _facade.TrySetCanceledAsync();

		public Task<bool> TrySetExceptionAsync(Exception exception) => _facade.TrySetExceptionAsync(exception);

		public bool TrySetResult() => _facade.TrySetResult(false);

		public bool TrySetCanceled() => _facade.TrySetCanceled();

		public bool TrySetException(Exception exception) => _facade.TrySetException(exception);

		protected virtual void Dispose(bool disposing)
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
			//GC.SuppressFinalize(this);
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
		private readonly CancellationTokenSource _cancel;
		private readonly CancellationTokenSource _icancel;
		private readonly CancellationToken _inner;
		private readonly bool _throwOnException;

		public MyTaskSource(CancellationToken token = default, bool throwOnException = true)
		{
			_lock = new();
			_source = new();
			_throwOnException = throwOnException;
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
			await using (var _ = await _lock.BlockAsyncLock().ConfigureAwait(false))
				_innerTask ??= InTask();

			return await _innerTask.ConfigureAwait(false);
		}

		private async Task<T> InTask()
		{
			await Task.WhenAny(_source.Task, Task.Delay(-1, _inner)).ConfigureAwait(false);
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			await TrySetCancAsyncInner().ConfigureAwait(false);

			var result = await _source.Task.ConfigureAwait(false);

			//If it's result, return.
			if (result.Item1)
				return (T)result.Item2;

			//Do a very very bad thing if we don't want exceptions
			if (!_throwOnException)
				return default;

			//Else throw exception.
			throw new MyTaskSourceException((Exception)result.Item2);
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
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);

			return !_inner.IsCancellationRequested && await Task.Run(() => _source.TrySetResult((true, result))).ConfigureAwait(false);
		}

		public async Task<bool> TrySetExceptionAsync(Exception result)
		{
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);

			return !_inner.IsCancellationRequested && await Task.Run(() => _source.TrySetResult((false, _throwOnException ? result : null))).ConfigureAwait(false);
		}

		public async Task<bool> TrySetCanceledAsync()
		{
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			return await TrySetCancAsyncInner().ConfigureAwait(false);
		}

		private async Task<bool> TrySetCancAsyncInner()
		{
			if (!_inner.IsCancellationRequested)
				await Task.Run(_cancel.Cancel).ConfigureAwait(false);

			return _inner.IsCancellationRequested && await Task.Run(() => _source.TrySetResult((false, _throwOnException ? new TaskCanceledException(null, null, _inner) : null))).ConfigureAwait(false);
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
			//Console.WriteLine($"Died-{GetType().Name}");
			disposedValue = true;
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			//GC.SuppressFinalize(this);
		}

		~MyTaskSource() => Dispose(disposing: false);
	}
}