namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// FIFO Fetch Blocking Async Collection | FIFOFBACollection
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FIFOFBACollection<T> : IDisposable
	{
		public FIFOFBACollection(bool configureAwait = false)
		{
			_sync = new();
			_chain = new();
			_chain.AddFirst((_generator = new()).MyTask);
			_configureAwait = configureAwait;
		}

		private readonly bool _configureAwait;
		private MyTaskSource<T> _generator;
		private readonly LinkedList<Task<T>> _chain;
		private readonly AsyncLocker _sync;

		public Task<bool> HasAny() => Task.FromResult(_chain.Any(x => x.IsCompleted));

		public async Task Handle(T stuff)
		{
			await using var _ = await _sync.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);

			if (_generator.MyTask.IsCanceled)
				await _generator.MyTask.ConfigureAwait(_configureAwait);

			await _generator.TrySetResultAsync(stuff).ConfigureAwait(_configureAwait);
			_chain.AddLast((_generator = new()).MyTask);
		}

		public async Task Cancel()
		{
			await using var _ = await _sync.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			await _generator.TrySetCanceledAsync().ConfigureAwait(_configureAwait);
		}

		public async Task<T> GetData(CancellationToken token = default)
		{
			Task<T> result = null;

			await using (var _ = await _sync.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
			{
				var node = _chain.First;
				result = node.Value;
				_chain.Remove(node);
			}

			using var revert = new MyTaskSource<T>(token);

			var either = await Task.WhenAny(result, revert.MyTask).ConfigureAwait(_configureAwait);

			if (either == revert.MyTask)
			{
				await using var _ = await _sync.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
				_chain.AddFirst(result);
				await revert.MyTask.ConfigureAwait(_configureAwait);
			}

			return await either.ConfigureAwait(_configureAwait);
		}

		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
				return;

			if (disposing)
			{
				_generator.Dispose();
				_generator = null;
				_sync.Dispose();
			}

			disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~FIFOFBACollection() => Dispose(false);
	}

	/// <summary>
	/// FIFO Fetch Blocking Async Collection | FIFOFBACollection
	/// </summary>
	public class FIFOFBACollection : IDisposable
	{
		public FIFOFBACollection() => _facade = new();

		private FIFOFBACollection<bool> _facade;

		public Task<bool> HasAny() => _facade.HasAny();

		public Task Handle() => _facade.Handle(true);

		public Task Cancel() => _facade.Cancel();

		public Task GetData(CancellationToken token = default) => _facade.GetData(token);

		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
				return;

			if (disposing)
				((IDisposable)_facade).Dispose();

			disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~FIFOFBACollection() => Dispose(false);
	}
}