namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// FIFO place, Take all, non-blocking async collection.
	/// </summary>
	public class FIFOPTACollection<T> : IDisposable
	{
		private readonly AsyncLocker _lock;
		private readonly LinkedList<T> _queue;
		private MyTaskSource _crank;
		private bool _disposedValue;
		private readonly bool _configureAwait;

		public FIFOPTACollection(bool configureAwait = false)
		{
			_configureAwait = configureAwait;
			_lock = new();
			_crank = new();
			_queue = new();
		}

		/// <summary>
		/// Appends item to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceLast(T item)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			_queue.AddLast(item);
			await _crank.TrySetResultAsync().ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Appends items to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceLast(IEnumerable<T> items)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			foreach (var item in items)
				_queue.AddLast(item);
			await _crank.TrySetResultAsync().ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Prepends item to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceFirst(T item)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			_queue.AddFirst(item);
			await _crank.TrySetResultAsync().ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Prepends items to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceFirst(IEnumerable<T> items)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			foreach (var item in items.Reverse())
				_queue.AddFirst(item);
			await _crank.TrySetResultAsync().ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Completes when anything has been placed.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task UntilPlaced(CancellationToken token = default)
		{
			Task task;

			await using (var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
				task = _crank.MyTask;

			await task.RelayAsync(token).ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Gets all items safely.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<IEnumerable<T>> GetAllSafe(CancellationToken token = default)
		{
			await UntilPlaced(token).ConfigureAwait(_configureAwait);
			return await GetAll().ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Gets all stored items at once.
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<T>> GetAll()
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			var outQueue = new List<T>(_queue.Count);

			while (_queue.Count > 0)
			{
				var node = _queue.First;
				outQueue.Add(node.Value);
				_queue.Remove(node);
			}
			await _crank.TrySetResultAsync().ConfigureAwait(_configureAwait);
			_crank = new();

			return outQueue;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue)
				return;

			if (disposing)
			{
				_lock.Dispose();
				_crank.Dispose();
				_crank = null;
			}

			_disposedValue = true;
		}

		~FIFOPTACollection() => Dispose(disposing: false);

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}