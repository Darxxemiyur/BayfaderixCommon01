using Name.Bayfaderix.Darxxemiyur.Common.Extensions;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// FIFO place, FIFO take, non-blocking async collection.
	/// </summary>
	public class FIFOPFIFOTCollection<T> : IDisposable, IAsyncDisposable
	{
		private readonly Queue<T> _queue;
		private readonly Queue<MyTaskSource<T>> _receivers;
		private readonly AsyncLocker _lock;
		private readonly bool _configureAwait;
		private bool _disposedValue;

		public FIFOPFIFOTCollection(bool configureAwait)
		{
			_configureAwait = configureAwait;
			_lock = new();
			_queue = new();
			_receivers = new();
		}

		private async Task InnerPlaceItem(T item)
		{
			if (_receivers.Count <= 0)
			{
				_queue.Enqueue(item);
				return;
			}

			var rem = _receivers.Dequeue();
			if (!await rem.TrySetResultAsync(item).ConfigureAwait(_configureAwait))
				await this.InnerPlaceItem(item).ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Place an item. If there are any receivers, hand it to them. If there aren't, place to
		/// the queue.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public async Task PlaceItem(T item)
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			await this.InnerPlaceItem(item).ConfigureAwait(_configureAwait);
		}

		/// <summary>
		/// Is there any items in the item queue
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AnyItems()
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			return _queue.Any();
		}

		/// <summary>
		/// Is there any receivers in the receiver queue
		/// </summary>
		/// <returns></returns>
		public async Task<bool> AnyReceivers()
		{
			await using var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			return _receivers.Any();
		}

		private async Task<Task<T>> InnerGetItem(CancellationToken token = default)
		{
			if (_queue.Count <= 0)
				return this.Enquer(token);

			var item = _queue.Dequeue();

			if (!token.IsCancellationRequested)
				return item == null ? await this.InnerGetItem(token).ConfigureAwait(_configureAwait) : Task.FromResult(item);

			_queue.Enqueue(item);

			return Task.FromCanceled<T>(token);
		}

		private async Task<T> Enquer(CancellationToken token)
		{
			var itemReceiver = new MyTaskSource<T>(token);
			_receivers.Enqueue(itemReceiver);

			return await itemReceiver.MyTask.ConfigureAwait(_configureAwait);
		}

		public ValueTask DisposeAsync() => new(MyTaskExtensions.RunOnScheduler(this.Dispose));

		/// <summary>
		/// Get item. If there is any, return it immediately, if not, take a place in the queue.
		/// </summary>
		/// <param name="token">Cancellation token</param>
		/// <returns>Task representing overall procedure of retrieving the item</returns>
		public async Task<T> GetItem(CancellationToken token = default)
		{
			Task<T> itemGet;
			await using (var _ = await _lock.BlockAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
				itemGet = await this.InnerGetItem(token).ConfigureAwait(_configureAwait);

			return await itemGet.ConfigureAwait(_configureAwait);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue)
				return;

			if (disposing)
			{
				_lock.Dispose();
			}

			_disposedValue = true;
		}

		~FIFOPFIFOTCollection() => this.Dispose(disposing: false);

		public void Dispose()
		{
			this.Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}