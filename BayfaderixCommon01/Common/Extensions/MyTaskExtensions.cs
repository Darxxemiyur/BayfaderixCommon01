namespace Name.Bayfaderix.Darxxemiyur.Common.Extensions
{
	public static class MyTaskExtensions
	{
		public static async Task<TResult?> AsType<TInput, TResult>(this Task<TInput> task) where TInput : class where TResult : class => await task.ConfigureAwait(false) as TResult;

		public static IEnumerable<Task<TResult?>> RunOnScheduler<TResult>(this IEnumerable<Func<CancellationToken, Task<TResult?>>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext));

		public static IEnumerable<Task<TResult?>> RunOnScheduler<TResult>(this IEnumerable<Func<Task<TResult?>>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler, continueOnCapturedContext));

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext);

		public static async Task<TResult?> RunOnScheduler<TResult>(this Func<Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false)
		{
			var placedTask = await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler ?? TaskScheduler.Current).ConfigureAwait(continueOnCapturedContext);
			return await placedTask.ConfigureAwait(continueOnCapturedContext);
		}

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<IAsyncRunnable> runnables, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(runnables.Select(x => (Func<CancellationToken, Task>)x.RunRunnable), token, scheduler, continueOnCapturedContext);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<CancellationToken, Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext));

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler, continueOnCapturedContext));

		public static Task RunOnScheduler(this Func<CancellationToken, Task> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext);

		public static async Task RunOnScheduler(this Func<Task> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false)
		{
			var placedTask = await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler ?? TaskScheduler.Current).ConfigureAwait(continueOnCapturedContext);
			await placedTask.ConfigureAwait(continueOnCapturedContext);
		}

		public static Func<Task<TResult?>> RunWithToken<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token) => () => func(token);

		public static Func<Task> RunWithToken(this Func<CancellationToken, Task> func, CancellationToken token) => () => func(token);
	}
}