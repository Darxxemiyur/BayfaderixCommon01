using Name.Bayfaderix.Darxxemiyur.Tasks;

namespace Name.Bayfaderix.Darxxemiyur.Async
{
	public sealed class AsyncRunnableOpBuilder : AsyncOpBuilderBase
	{
		private IAsyncRunnable? _asyncRunnable;

		public override AsyncOpBuilderKind Kind => AsyncOpBuilderKind.AsyncRunnable;

		public IAsyncRunnable? GetAsyncRunnable() => _asyncRunnable;

		public AsyncRunnableOpBuilder() => _asyncRunnable = null;

		public AsyncRunnableOpBuilder(AsyncRunnableOpBuilder oop) : base(oop) => _asyncRunnable = oop._asyncRunnable;

		public AsyncRunnableOpBuilder OnAsyncRunnable(Action<IAsyncRunnable?> action)
		{
			action(_asyncRunnable);
			return this;
		}

		public AsyncRunnableOpBuilder WithAsyncRunnable(IAsyncRunnable? runnable)
		{
			_asyncRunnable = runnable;
			return this;
		}

		public AsyncRunnableOpBuilder WithNoAsyncRunnable()
		{
			_asyncRunnable = null;
			return this;
		}

		internal override async Task Start(CancellationToken token = default)
		{
			if (_asyncRunnable == null)
				return;

			using var conf = this.GetAsyncOpBatch(token);
			var tokenU = conf.Token;
			var factory = conf.TaskFactory;

			await factory.StartNew(() => _asyncRunnable.RunRunnable(conf.Token), tokenU).Unwrap().ConfigureAwait(ConfigureAwait);
		}
	}
}
