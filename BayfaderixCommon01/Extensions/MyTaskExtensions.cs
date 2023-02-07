using Name.Bayfaderix.Darxxemiyur.Common.Async;

namespace Name.Bayfaderix.Darxxemiyur.Common.Extensions
{
	public static class MyTaskExtensions
	{
		public static async Task<TResult?> AsType<TInput, TResult>(this Task<TInput> task, bool configureAwait = false) where TInput : class where TResult : class => await task.ConfigureAwait(configureAwait) as TResult;

		public static IEnumerable<Task<TResult?>> RunOnScheduler<TResult>(this IEnumerable<Func<CancellationToken, Task<TResult?>>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext));

		public static IEnumerable<Task<TResult?>> RunOnScheduler<TResult>(this IEnumerable<Func<Task<TResult?>>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler, continueOnCapturedContext));

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<CancellationToken, TResult?> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext);

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext);

		private static async Task<TaskScheduler> GetScheduler(TaskScheduler? suggested)
		{
			if (suggested != null)
				return suggested;

			if (SynchronizationContext.Current is MySingleThreadSyncContext context)
				return await context.MyTaskScheduler;

			//TaskScheduler.FromCurrentSynchronizationContext() with current as null would probably return TaskScheduler.Default, but I'm not riskin it.
			if (SynchronizationContext.Current is not null)
				return TaskScheduler.FromCurrentSynchronizationContext();

			return TaskScheduler.Current;
		}

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<TResult?> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(async () => func(), token, scheduler, continueOnCapturedContext);

		public static async Task<TResult?> RunOnScheduler<TResult>(this Func<Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = await GetScheduler(scheduler)).Unwrap().ContinueWith(x => x.Result, token, default, scheduler).ConfigureAwait(continueOnCapturedContext);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<IAsyncRunnable> runnables, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(runnables.Select(x => (Func<CancellationToken, Task>)x.RunRunnable), token, scheduler, continueOnCapturedContext);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<CancellationToken, Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext));

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler, continueOnCapturedContext));

		public static Task RunOnScheduler(this Func<CancellationToken, Task> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext);

		public static Task RunOnScheduler(this Action func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(async () => func(), token, scheduler, continueOnCapturedContext);

		public static async Task RunOnScheduler(this Func<Task> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = await GetScheduler(scheduler)).Unwrap().ContinueWith(x => { }, token, default, scheduler).ConfigureAwait(continueOnCapturedContext);

		public static Func<Task<TResult?>> RunWithToken<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token) => () => func(token);

		public static Func<Task> RunWithToken(this Func<CancellationToken, Task> func, CancellationToken token) => () => func(token);
	}
}