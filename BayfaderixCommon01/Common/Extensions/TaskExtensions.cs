namespace Name.Bayfaderix.Darxxemiyur.Common.Extensions
{
	public static class TaskExtensions
	{
		public static async Task<TResult?> AsType<TInput, TResult>(this Task<TInput> task) where TInput : class where TResult : class => await task as TResult;
	}
}