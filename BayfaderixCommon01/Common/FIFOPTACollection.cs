namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// FIFO place, Take all, non-blocking async collection.
	/// </summary>
	public class FIFOPTACollection<T>
	{
		private readonly AsyncLocker _lock;
		private readonly Queue<T> _queue;
		private MyTaskSource _crank;

		public FIFOPTACollection()
		{
			_lock = new();
			_crank = new();
			_queue = new();
		}

		public async Task Place(T item)
		{
			await using var _ = await _lock.BlockAsyncLock();
			_queue.Enqueue(item);
			await _crank.TrySetResultAsync();
		}

		public async Task Place(IEnumerable<T> items)
		{
			await using var _ = await _lock.BlockAsyncLock();
			foreach (var item in items)
				_queue.Enqueue(item);
			await _crank.TrySetResultAsync();
		}

		/// <summary>
		/// Completes when anything has been placed.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task UntilPlaced(CancellationToken token = default)
		{
			Task task;

			await using (var _ = await _lock.BlockAsyncLock())
				task = _crank.MyTask;

			await task.RelayAsync(token);
		}

		/// <summary>
		/// Gets all items safely.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public async Task<IEnumerable<T>> GetAllSafe(CancellationToken token = default)
		{
			await UntilPlaced(token);
			return await GetAll();
		}

		public async Task<IEnumerable<T>> GetAll()
		{
			await using var _ = await _lock.BlockAsyncLock();
			var outQueue = new List<T>();

			while (_queue.Count > 0)
				outQueue.Add(_queue.Dequeue());

			await _crank.TrySetCanceledAsync();
			_crank = new();
			return outQueue;
		}
	}
}