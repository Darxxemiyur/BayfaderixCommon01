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

		private static async Task<TaskScheduler> GetScheduler(TaskScheduler? suggested, bool configureAwait = false)
		{
			if (suggested != null)
				return suggested;

			if (SynchronizationContext.Current is MySingleThreadSyncContext context)
				return await context.MyTaskScheduler.ConfigureAwait(configureAwait);

			//TaskScheduler.FromCurrentSynchronizationContext() with current as null would probably return TaskScheduler.Default, but I'm not riskin it.
			if (SynchronizationContext.Current is not null)
				return TaskScheduler.FromCurrentSynchronizationContext();

			return TaskScheduler.Current;
		}

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<TResult?> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => Task.FromResult(func()), token, scheduler, continueOnCapturedContext);

		public static async Task<TResult?> RunOnScheduler<TResult>(this Func<Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = await GetScheduler(scheduler, continueOnCapturedContext)).Unwrap().ContinueWith(x => x.Result, token, default, scheduler).ConfigureAwait(continueOnCapturedContext);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<IAsyncRunnable> runnables, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(runnables.Select(x => (Func<CancellationToken, Task>)x.RunRunnable), token, scheduler, continueOnCapturedContext);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<CancellationToken, Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext));

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler, continueOnCapturedContext));

		public static Task RunOnScheduler(this Func<CancellationToken, Task> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => RunOnScheduler(() => func(token), token, scheduler, continueOnCapturedContext);

		public static async Task RunOnScheduler(this Action func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = await GetScheduler(scheduler, continueOnCapturedContext)).ContinueWith(x => { }, token, default, scheduler).ConfigureAwait(continueOnCapturedContext);

		public static async Task RunOnScheduler(this Func<Task> func, CancellationToken token = default, TaskScheduler? scheduler = default, bool continueOnCapturedContext = false) => await Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = await GetScheduler(scheduler, continueOnCapturedContext)).Unwrap().ContinueWith(x => { }, token, default, scheduler).ConfigureAwait(continueOnCapturedContext);

		public static async Task MyContinueWith(this Task func, Action<Task> act, TaskScheduler? scheduler = default, bool configureAwait = false) => await func.ContinueWith(act, await GetScheduler(scheduler, configureAwait).ConfigureAwait(configureAwait)).ConfigureAwait(configureAwait);

		public static async Task MyContinueWith<TResult>(this Task<TResult> func, Action<Task<TResult>> act, TaskScheduler? scheduler = default, bool configureAwait = false) => await func.ContinueWith(act, await GetScheduler(scheduler, configureAwait).ConfigureAwait(configureAwait)).ConfigureAwait(configureAwait);

		public static async Task<TResult> MyContinueWith<TResult>(this Task func, Func<Task, TResult> act, TaskScheduler? scheduler = default, bool configureAwait = false) => await func.ContinueWith(act, await GetScheduler(scheduler, configureAwait).ConfigureAwait(configureAwait)).ConfigureAwait(configureAwait);

		public static async Task<TOutResult> MyContinueWith<TInResult, TOutResult>(this Task<TInResult> func, Func<Task<TInResult>, TOutResult> act, TaskScheduler? scheduler = default, bool configureAwait = false) => await func.ContinueWith(act, await GetScheduler(scheduler, configureAwait).ConfigureAwait(configureAwait)).ConfigureAwait(configureAwait);

		public static Func<Task<TResult?>> RunWithToken<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token) => () => func(token);

		public static Func<Task> RunWithToken(this Func<CancellationToken, Task> func, CancellationToken token) => () => func(token);
	}
}