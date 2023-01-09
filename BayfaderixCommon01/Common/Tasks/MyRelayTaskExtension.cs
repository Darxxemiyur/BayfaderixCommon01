namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public static class ExtensionsForMyRelayTask
	{
		public static Task RelayAsync(this Task me, CancellationToken token = default) => new MyRelayTask(me, token).TheTask;

		public static async Task RelayAsync(this Task me, TimeSpan timeout, CancellationToken token = default)
		{
			using var ttokenS = new CancellationTokenSource(timeout);
			using var rtokenS = CancellationTokenSource.CreateLinkedTokenSource(token, ttokenS.Token);

			await me.RelayAsync(rtokenS.Token);
		}

		public static Task<T> RelayAsync<T>(this Task<T> me, CancellationToken token = default) => new MyRelayTask<T>(me, token).TheTask;

		public static async Task<T> RelayAsync<T>(this Task<T> me, TimeSpan timeout, CancellationToken token = default)
		{
			using var ttokenS = new CancellationTokenSource(timeout);
			using var rtokenS = CancellationTokenSource.CreateLinkedTokenSource(token, ttokenS.Token);

			return await me.RelayAsync(rtokenS.Token);
		}
	}
}