namespace Name.Bayfaderix.Darxxemiyur.Common
{
	/// <summary>
	/// Execution "situation" that this job wants to be in.
	/// </summary>
	public enum AsyncJobType
	{
		/// <summary>
		/// Run Job in thread pool.
		/// </summary>
		Pooled,

		/// <summary>
		/// Run separate thread for job.
		/// </summary>
		UniqueThreaded,

		/// <summary>
		/// Run in async job manager's own sub-thread
		/// </summary>
		SubThreaded,

		/// <summary>
		/// Job runs in the same thread
		/// </summary>
		Inline,
	}
}