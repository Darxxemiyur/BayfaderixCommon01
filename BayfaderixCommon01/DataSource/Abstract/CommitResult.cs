namespace Name.Bayfaderix.Darxxemiyur.DataSource.Abstract;

/// <summary>
/// Commit result flags.
/// </summary>
public enum CommitResult
{
	/// <summary>
	/// Commiting changes has failed.
	/// </summary>
	Failed,
	/// <summary>
	/// Changes have been successfully commited.
	/// </summary>
	Commited,
	/// <summary>
	/// No changes to commit.
	/// </summary>
	NoChanges,
	/// <summary>
	/// Commit aborted due to data changes present in the source.
	/// </summary>
	Aborted,
	/// <summary>
	/// Commit failed due to data changes in the source.
	/// </summary>
	Conflict,
}
