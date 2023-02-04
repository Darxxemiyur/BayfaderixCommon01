namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// FIFO place, Take all, non-blocking async collection.
	/// </summary>
	public class FIFOPTACollection<T>
	{
		private readonly AsyncLocker _lock;
		private readonly LinkedList<T> _queue;
		private MyTaskSource _crank;

		public FIFOPTACollection()
		{
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
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			_queue.AddLast(item);
			await _crank.TrySetResultAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Appends items to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceLast(IEnumerable<T> items)
		{
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			foreach (var item in items)
				_queue.AddLast(item);
			await _crank.TrySetResultAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Prepends item to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceFirst(T item)
		{
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			_queue.AddFirst(item);
			await _crank.TrySetResultAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Prepends items to the list
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public async Task PlaceFirst(IEnumerable<T> items)
		{
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			foreach (var item in items.Reverse())
				_queue.AddFirst(item);
			await _crank.TrySetResultAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Completes when anything has been placed.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task UntilPlaced(CancellationToken token = default)
		{
			Task task;

			await using (var _ = await _lock.BlockAsyncLock().ConfigureAwait(false))
				task = _crank.MyTask;

			await task.RelayAsync(token).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets all items safely.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<IEnumerable<T>> GetAllSafe(CancellationToken token = default)
		{
			await UntilPlaced(token).ConfigureAwait(false);
			return await GetAll().ConfigureAwait(false);
		}

		/// <summary>
		/// Gets all stored items at once.
		/// </summary>
		/// <returns></returns>
		public async Task<IEnumerable<T>> GetAll()
		{
			await using var _ = await _lock.BlockAsyncLock().ConfigureAwait(false);
			var outQueue = new List<T>(_queue.Count);

			while (_queue.Count > 0)
			{
				var node = _queue.First;
				outQueue.Add(node.Value);
				_queue.Remove(node);
			}
			await _crank.TrySetResultAsync().ConfigureAwait(false);
			_crank = new();
			return outQueue;
		}
	}
}