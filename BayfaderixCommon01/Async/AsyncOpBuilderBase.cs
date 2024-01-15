namespace Name.Bayfaderix.Darxxemiyur.Async
{
	internal record class AsyncOpBatch(TaskFactory TaskFactory, CancellationTokenSource TokenSource) : IDisposable
	{
		private bool disposedValue;

		public CancellationToken Token => TokenSource.Token;

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
				return;

			TokenSource.Dispose();

			disposedValue = true;
		}

		~AsyncOpBatch() => this.Dispose(disposing: false);

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

	public enum AsyncOpBuilderKind
	{
		Batch,
		Delegate,
		AsyncRunnable,
	}

	public abstract class AsyncOpBuilderBase
	{
		internal abstract Task Start(CancellationToken token = default);

		public abstract AsyncOpBuilderKind Kind
		{
			get;
		}

		public static TaskScheduler GetScheduler(TaskScheduler? suggested = default)
		{
			if (suggested != null)
				return suggested;

			if (SynchronizationContext.Current is IMyUnderlyingContext context && context.MyTaskScheduler != null)
				return context.MyTaskScheduler;

			//TaskScheduler.FromCurrentSynchronizationContext() with current as null would probably return TaskScheduler.Default, but I'm not riskin it.
			if (SynchronizationContext.Current is not null)
				return TaskScheduler.FromCurrentSynchronizationContext();

			return TaskScheduler.Current;
		}

		protected CancellationToken? Token
		{
			get; private set;
		}

		protected TaskCreationOptions? CreationOptions
		{
			get; private set;
		}

		protected TaskContinuationOptions? ContinuationOptions
		{
			get; private set;
		}

		protected TaskFactory? Factory
		{
			get; private set;
		}

		protected TaskScheduler? Scheduler
		{
			get; private set;
		}

		protected bool ConfigureAwait
		{
			get; private set;
		}

		protected AsyncOpBuilderBase()
		{
		}

		protected AsyncOpBuilderBase(AsyncOpBuilderBase oop)
		{
			Factory = oop.Factory;
			Scheduler = oop.Scheduler;
			CreationOptions = oop.CreationOptions;
			ContinuationOptions = oop.ContinuationOptions;
			ConfigureAwait = oop.ConfigureAwait;
		}

		public AsyncOpBuilderBase WithTaskFactory(TaskFactory? factory)
		{
			Factory = factory;
			return this;
		}

		public AsyncOpBuilderBase WithDefaultFactory() => throw new NotImplementedException();

		public AsyncOpBuilderBase WithCurrentFactory() => throw new NotImplementedException();

		public AsyncOpBuilderBase WithNoFactory()
		{
			Factory = null;
			return this;
		}

		public AsyncOpBuilderBase WithScheduler(TaskScheduler? scheduler)
		{
			Scheduler = scheduler;
			return this;
		}

		public AsyncOpBuilderBase WithScheduler(IMyUnderlyingContext? context) => this.WithScheduler(context?.MyTaskScheduler);

		public AsyncOpBuilderBase WithDefaultScheduler() => this.WithScheduler(GetScheduler());

		public AsyncOpBuilderBase WithCurrentScheduler() => this.WithScheduler(TaskScheduler.Current);

		/// <summary>
		/// Attempts to select <see cref="MySingleThreadSyncContext"/>'s Task scheduler. <br/>
		/// TODO: make it actually work as intended.
		/// </summary>
		/// <returns></returns>
		public AsyncOpBuilderBase TryWithMyScheduler() => GetScheduler() is var scheduler ? WithScheduler(scheduler) : this;

		public AsyncOpBuilderBase WithNoScheduler()
		{
			Scheduler = null;
			return this;
		}

		public AsyncOpBuilderBase WithCancellationToken(CancellationToken? token)
		{
			Token = token;
			return this;
		}

		public AsyncOpBuilderBase WithNoToken()
		{
			Token = null;
			return this;
		}

		public AsyncOpBuilderBase WithCreationOptions(TaskCreationOptions? options)
		{
			CreationOptions = options;
			return this;
		}

		public AsyncOpBuilderBase WithContinuationOptions(TaskContinuationOptions? options)
		{
			ContinuationOptions = options;
			return this;
		}

		public AsyncOpBuilderBase WithNoCreationOptions() => this.WithCreationOptions(null);

		public AsyncOpBuilderBase WithNoContinuationOptions() => this.WithContinuationOptions(null);

		public AsyncOpBuilderBase WithConfigureAwait(bool configureAwait)
		{
			ConfigureAwait = configureAwait;
			return this;
		}

		internal AsyncOpBatch GetAsyncOpBatch(CancellationToken token = default)
		{
			var ts = CancellationTokenSource.CreateLinkedTokenSource(Token ?? default, token);
			var sched = Factory?.Scheduler ?? Scheduler;
			var cropts = (CreationOptions ?? TaskCreationOptions.None) | (Factory?.CreationOptions ?? TaskCreationOptions.None);
			var cnopts = (ContinuationOptions ?? TaskContinuationOptions.None) | (Factory?.ContinuationOptions ?? TaskContinuationOptions.None);
			var factory = new TaskFactory(ts.Token, cropts, cnopts, sched);

			return new AsyncOpBatch(factory, ts);
		}
	}
}
