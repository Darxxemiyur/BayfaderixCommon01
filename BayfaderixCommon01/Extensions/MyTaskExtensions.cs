using Name.Bayfaderix.Darxxemiyur.Common.Async;

namespace Name.Bayfaderix.Darxxemiyur.Common.Extensions
{
	public static class MyTaskExtensions
	{
		public static async Task<TResult?> AsType<TInput, TResult>(this Task<TInput> task, bool configureAwait = false) where TInput : class where TResult : class => await task.ConfigureAwait(configureAwait) as TResult;

		public static IEnumerable<Task<TResult?>> RunOnScheduler<TResult>(this IEnumerable<Func<CancellationToken, Task<TResult?>>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler));

		public static IEnumerable<Task<TResult?>> RunOnScheduler<TResult>(this IEnumerable<Func<Task<TResult?>>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler));

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<CancellationToken, TResult?> func, CancellationToken token = default, TaskScheduler? scheduler = default) => RunOnScheduler(() => func(token), token, scheduler);

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default) => RunOnScheduler(() => func(token), token, scheduler);

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

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<TResult?> func, CancellationToken token = default, TaskScheduler? scheduler = default) => RunOnScheduler(() => Task.FromResult(func()), token, scheduler);

		public static Task<TResult?> RunOnScheduler<TResult>(this Func<Task<TResult?>> func, CancellationToken token = default, TaskScheduler? scheduler = default) => Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = GetScheduler(scheduler)).Unwrap().ContinueWith(x => x.Result, token, default, scheduler);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<IAsyncRunnable> runnables, CancellationToken token = default, TaskScheduler? scheduler = default) => runnables.Select(x => RunOnScheduler(x, token, scheduler));

		public static Task RunOnScheduler(this IAsyncRunnable runnable, CancellationToken token = default, TaskScheduler? scheduler = default) => RunOnScheduler(runnable.RunRunnable, token, scheduler);

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<CancellationToken, Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default) => funcs.Select(func => RunOnScheduler(() => func(token), token, scheduler));

		public static IEnumerable<Task> RunOnScheduler(this IEnumerable<Func<Task>> funcs, CancellationToken token = default, TaskScheduler? scheduler = default) => funcs.Select(func => RunOnScheduler(() => func(), token, scheduler));

		public static Task RunOnScheduler(this Func<CancellationToken, Task> func, CancellationToken token = default, TaskScheduler? scheduler = default) => RunOnScheduler(() => func(token), token, scheduler);

		public static Task RunOnScheduler(this Action func, CancellationToken token = default, TaskScheduler? scheduler = default) => Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = GetScheduler(scheduler)).ContinueWith(x => { }, token, default, scheduler);

		public static Task RunOnScheduler(this Func<Task> func, CancellationToken token = default, TaskScheduler? scheduler = default) => Task.Factory.StartNew(func, token, TaskCreationOptions.None, scheduler = GetScheduler(scheduler)).Unwrap().ContinueWith(x => { }, token, default, scheduler);

		public static Task MyContinueWith(this Task func, Action<Task> act, TaskScheduler? scheduler = default) => func.ContinueWith(act, GetScheduler(scheduler));

		public static Task MyContinueWith<TResult>(this Task<TResult> func, Action<Task<TResult>> act, TaskScheduler? scheduler = default) => func.ContinueWith(act, GetScheduler(scheduler));

		public static Task<TResult> MyContinueWith<TResult>(this Task func, Func<Task, TResult> act, TaskScheduler? scheduler = default) => func.ContinueWith(act, GetScheduler(scheduler));

		public static Task<TOutResult> MyContinueWith<TInResult, TOutResult>(this Task<TInResult> func, Func<Task<TInResult>, TOutResult> act, TaskScheduler? scheduler = default) => func.ContinueWith(act, GetScheduler(scheduler));

		public static Func<Task<TResult?>> RunWithToken<TResult>(this Func<CancellationToken, Task<TResult?>> func, CancellationToken token) => () => func(token);

		public static Func<Task> RunWithToken(this Func<CancellationToken, Task> func, CancellationToken token) => () => func(token);
	}
}