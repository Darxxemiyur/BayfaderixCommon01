namespace Name.Bayfaderix.Darxxemiyur.Async;

public sealed class BatchAsyncOpBuilder : AsyncOpBuilderBase
{
	private readonly LinkedList<AsyncOpBuilderBase> _asyncOpBuilders;
	public ICollection<AsyncOpBuilderBase> AsyncOpBuilders => _asyncOpBuilders;

	public override AsyncOpBuilderKind Kind => AsyncOpBuilderKind.Batch;

	public BatchAsyncOpBuilder() => _asyncOpBuilders = new();

	public BatchAsyncOpBuilder(BatchAsyncOpBuilder oop) : base(oop) => _asyncOpBuilders = oop._asyncOpBuilders;

	public BatchAsyncOpBuilder WithDelegateAsyncOp(Action<DelegateAsyncOpBuilder> action)
	{
		var del = new DelegateAsyncOpBuilder();
		action(del);
		return this.WithDelegateAsyncOp(del);
	}

	public BatchAsyncOpBuilder WithDelegateAsyncOp(DelegateAsyncOpBuilder del)
	{
		_asyncOpBuilders.AddLast(del);
		return this;
	}

	public BatchAsyncOpBuilder WithRunnableAsyncOp(Action<AsyncRunnableOpBuilder> action)
	{
		var del = new AsyncRunnableOpBuilder();
		action(del);
		return this.WithRunnableAsyncOp(del);
	}

	public BatchAsyncOpBuilder WithRunnableAsyncOp(AsyncRunnableOpBuilder del)
	{
		_asyncOpBuilders.AddLast(del);
		return this;
	}

	public BatchAsyncOpBuilder WithBatchAsyncOp(Action<BatchAsyncOpBuilder> action)
	{
		var del = new BatchAsyncOpBuilder();
		action(del);
		return this.WithBatchAsyncOp(del);
	}

	public BatchAsyncOpBuilder WithBatchAsyncOp(BatchAsyncOpBuilder del)
	{
		_asyncOpBuilders.AddLast(del);
		return this;
	}

	public IEnumerable<Task> Run(CancellationToken token = default)
	{
		var conf = this.GetAsyncOpBatch(token);
		var tokenU = conf.Token;

		foreach (var task in _asyncOpBuilders)
			yield return conf.TaskFactory.StartNew(() => task.Start(tokenU), tokenU).Unwrap();
	}

	internal override Task Start(CancellationToken token = default) => Task.WhenAll(this.Run(token));
}
