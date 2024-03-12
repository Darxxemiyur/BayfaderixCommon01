using Name.Bayfaderix.Darxxemiyur.Extensions;
using Name.Bayfaderix.Darxxemiyur.Tasks;

using System.Runtime.CompilerServices;

namespace Name.Bayfaderix.Darxxemiyur.Collections;

/// <summary>
/// FIFO Fetch Blocking Async Collection | FIFOFBACollection
/// </summary>
/// <typeparam name="T"></typeparam>
public class FIFOFBACollection<T> : IDisposable, IAsyncDisposable, IAsyncEnumerable<T>
{
	public FIFOFBACollection(bool configureAwait = false)
	{
		_sync = new();
		_chain = new();
		_chain.AddFirst((_generator = new()).Task);
		_configureAwait = configureAwait;
	}

	private readonly bool _configureAwait;
	private TaskCompletionSource<T> _generator;
	private readonly LinkedList<Task<T>> _chain;
	private readonly AsyncLocker _sync;

	public Task<bool> HasAny() => Task.FromResult(_chain.Any(x => x.IsCompleted));

	public async Task Handle(T stuff)
	{
		using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);

		if (_generator.Task.IsCanceled)
			await _generator.Task.ConfigureAwait(_configureAwait);

		_generator.TrySetResult(stuff);
		_chain.AddLast((_generator = new()).Task);
	}

	public async Task Cancel()
	{
		using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
		_generator.TrySetCanceled();
	}

	public async Task<T> GetData(CancellationToken token = default)
	{
		LinkedListNode<Task<T>> result;

		using (var _ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait))
			result = _chain.First!;

		var source = new TaskCompletionSource<T>();
		using var reg = token.Register(() => source.TrySetResult(default));

		var either = await Task.WhenAny(result.Value, source.Task).ConfigureAwait(_configureAwait);

		if (either == result.Value)
		{
			using var __ = await _sync.ScopeAsyncLock(default, _configureAwait).ConfigureAwait(_configureAwait);
			if (result.List == _chain)
				_chain.Remove(result);
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
			_generator = null;
			_sync.Dispose();
		}

		disposedValue = true;
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	private async IAsyncEnumerable<T> AsAsyncEnumerable([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		while (await this.HasAny() && !cancellationToken.IsCancellationRequested)
			yield return await this.GetData(cancellationToken);
	}

	public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => this.AsAsyncEnumerable(cancellationToken).GetAsyncEnumerator(cancellationToken);

	public ValueTask DisposeAsync() => new(MyTaskExtensions.RunOnScheduler(this.Dispose));

	~FIFOFBACollection() => this.Dispose(false);
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
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	~FIFOFBACollection() => this.Dispose(false);
}
