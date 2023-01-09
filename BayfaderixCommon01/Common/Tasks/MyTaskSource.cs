namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class MyTaskSource : IDisposable
	{
		private readonly MyTaskSource<bool> _facade;
		private bool disposedValue;

		public MyTaskSource(CancellationToken token = default) => _facade = new(token);

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
		private readonly CancellationTokenSource _cancel;
		private readonly CancellationToken _inner;

		public MyTaskSource(CancellationToken token = default)
		{
			_lock = new();
			_source = new();
			_cancel = new CancellationTokenSource();
			_inner = CancellationTokenSource.CreateLinkedTokenSource(token, _cancel.Token).Token;
		}

		private Task<T> _innerTask;
		private bool disposedValue;

		public Task<T> MyTask => InSecure();

		public static implicit operator Task<T>(MyTaskSource<T> task) => task.MyTask;

		private async Task<T> InSecure()
		{
			await using (var _ = await _lock.BlockAsyncLock())
				_innerTask ??= InTask();

			return await _innerTask;
		}

		private async Task<T> InTask()
		{
			await Task.WhenAny(_source.Task, Task.Delay(-1, _inner));
			await using var _ = await _lock.BlockAsyncLock();
			await TrySetCancAsyncInner();

			var result = await _source.Task;

			//If it's result, return.
			if (result.Item1)
				return (T)result.Item2;

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

			return !_inner.IsCancellationRequested && _source.TrySetResult((false, result));
		}

		public bool TrySetCanceled()
		{
			using var _ = _lock.BlockLock();

			if (!_inner.IsCancellationRequested)
				_cancel.Cancel();

			return _inner.IsCancellationRequested && _source.TrySetResult((false, new TaskCanceledException(null, null, _inner)));
		}

		/// <summary>
		/// Tries to set result. True if success, False if failure. Will fail if was cancelled.
		/// </summary>
		/// <param name="result"></param>
		/// <returns></returns>
		public async Task<bool> TrySetResultAsync(T result)
		{
			await using var _ = await _lock.BlockAsyncLock();

			return !_inner.IsCancellationRequested && await Task.Run(() => _source.TrySetResult((true, result)));
		}

		public async Task<bool> TrySetExceptionAsync(Exception result)
		{
			await using var _ = await _lock.BlockAsyncLock();

			return !_inner.IsCancellationRequested && await Task.Run(() => _source.TrySetResult((false, result)));
		}

		public async Task<bool> TrySetCanceledAsync()
		{
			await using var _ = await _lock.BlockAsyncLock();
			return await TrySetCancAsyncInner();
		}

		private async Task<bool> TrySetCancAsyncInner()
		{
			if (!_inner.IsCancellationRequested)
				await Task.Run(_cancel.Cancel);

			return _inner.IsCancellationRequested && await Task.Run(() => _source.TrySetResult((false, new TaskCanceledException(null, null, _inner))));
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
				return;

			if (disposing)
			{
				_lock.Dispose();
				_cancel.Dispose();
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