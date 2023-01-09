namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public interface IAsyncJobManager : IAsyncRunnable
	{
		Task<AsyncJob> AddNew(AsyncJob job);

		Task<IEnumerable<AsyncJob>> AddNew(IEnumerable<AsyncJob> runners);

		Task<IEnumerable<AsyncJob>> AddNew(params AsyncJob[] runners);
	}
}