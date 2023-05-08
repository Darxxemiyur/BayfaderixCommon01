using Name.Bayfaderix.Darxxemiyur.Tasks;

namespace Name.Bayfaderix.Darxxemiyur.Async
{
	public sealed class DelegateAsyncOpBuilder : AsyncOpBuilderBase
	{
		private enum DelegateKind
		{
			Unset,
			NoTokenNoTask,
			TokenNoTask,
			NoTokenTask,
			TokenTask,
			NoTokenTaskResult,
			TokenTaskResult,
		}

		public override AsyncOpBuilderKind Kind => AsyncOpBuilderKind.Delegate;
		private DelegateKind _kind;
		private Func<CancellationToken, Task>? _tokenTask;
		private Func<Task>? _noTokenTask;
		private Action<CancellationToken>? _tokenNoTask;
		private Action? _noTokenNoTask;

		public DelegateAsyncOpBuilder() => this.WithNoDelegate();

		public DelegateAsyncOpBuilder(DelegateAsyncOpBuilder oop) : base(oop)
		{
			_kind = oop._kind;
			_tokenTask = oop._tokenTask;
			_noTokenTask = oop._noTokenTask;
			_tokenNoTask = oop._tokenNoTask;
			_noTokenNoTask = oop._noTokenNoTask;
		}

		public DelegateAsyncOpBuilder WithDelegate(Func<CancellationToken, Task> func)
		{
			_kind = DelegateKind.TokenTask;
			_tokenTask = func;
			_noTokenTask = null;
			_tokenNoTask = null;
			_noTokenNoTask = null;
			return this;
		}

		public DelegateAsyncOpBuilder WithDelegate(Func<Task> func)
		{
			_kind = DelegateKind.NoTokenTask;
			_tokenTask = null;
			_noTokenTask = func;
			_tokenNoTask = null;
			_noTokenNoTask = null;
			return this;
		}

		public DelegateAsyncOpBuilder WithDelegate(Action<CancellationToken> func)
		{
			_kind = DelegateKind.TokenNoTask;
			_tokenTask = null;
			_noTokenTask = null;
			_tokenNoTask = func;
			_noTokenNoTask = null;
			return this;
		}

		public DelegateAsyncOpBuilder WithDelegate(Action func)
		{
			_kind = DelegateKind.NoTokenNoTask;
			_tokenTask = null;
			_noTokenTask = null;
			_tokenNoTask = null;
			_noTokenNoTask = func;
			return this;
		}

		/// <summary>
		/// Uses ConfigureAwait and Token that are configured. Builder with this is not affected by token in <see cref="Start(CancellationToken)"/>
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="delegate"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public DelegateAsyncOpBuilder WithDelegate<TResult>(Func<Task<TResult>> @delegate, out Task<TResult> result)
		{
			var source = new MyTaskSource<TResult>(Token ?? default, true, ConfigureAwait);
			this.WithDelegate(() => source.MimicResult(@delegate()));
			_kind = DelegateKind.NoTokenTaskResult;
			result = source.MyTask;
			return this;
		}

		/// <summary>
		/// Uses ConfigureAwait and Token that are configured. Builder with this is not affected by token in <see cref="Start(CancellationToken)"/>
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="delegate"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public DelegateAsyncOpBuilder WithDelegate<TResult>(Func<CancellationToken, Task<TResult>> @delegate, out Task<TResult> result)
		{
			var source = new MyTaskSource<TResult>(Token ?? default, true, ConfigureAwait);
			this.WithDelegate((x) => source.MimicResult(@delegate(x)));
			_kind = DelegateKind.TokenTaskResult;
			result = source.MyTask;
			return this;
		}

		public DelegateAsyncOpBuilder WithNoDelegate()
		{
			_kind = DelegateKind.Unset;
			_tokenTask = null;
			_noTokenTask = null;
			_tokenNoTask = null;
			_noTokenNoTask = null;
			return this;
		}

		internal override async Task Start(CancellationToken token = default)
		{
			using var conf = this.GetAsyncOpBatch(token);
			var tokenU = conf.Token;
			var factory = conf.TaskFactory;
			switch (_kind)
			{
				case DelegateKind.NoTokenNoTask:
					await factory.StartNew(_noTokenNoTask, tokenU).ConfigureAwait(ConfigureAwait);
					break;

				case DelegateKind.TokenNoTask:
					await factory.StartNew(() => _tokenNoTask(tokenU), tokenU).ConfigureAwait(ConfigureAwait);
					break;

				case DelegateKind.NoTokenTask:
				case DelegateKind.NoTokenTaskResult:
					await factory.StartNew(_noTokenTask, tokenU).Unwrap().ConfigureAwait(ConfigureAwait);
					break;

				case DelegateKind.TokenTask:
				case DelegateKind.TokenTaskResult:
					await factory.StartNew(() => _tokenTask(tokenU), tokenU).Unwrap().ConfigureAwait(ConfigureAwait);
					break;
			}
		}
	}
}
