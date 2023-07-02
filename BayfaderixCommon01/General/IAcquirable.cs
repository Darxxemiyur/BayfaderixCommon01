using System.Linq.Expressions;

namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// Presents an entity that can be asynchroniously acquired.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAcquirable<T> where T : class
{
	/// <summary>
	/// Primarily intended to be used with optional configuring.
	/// </summary>
	IMessageCommunicable? QnA => new StupidMessageCommunicable();

	/// <summary>
	/// Ask instance if it knows how to acquire the entity.
	/// </summary>
	/// <returns>Returns true if it knows how to acquire the entity. False otherwise.</returns>
	Task<bool> IsAcquirable();

	/// <summary>
	/// Attempt to acquire the entity.
	/// </summary>
	/// <returns>Entity if any.</returns>
	Task<T?> Acquire();

	/// <summary>
	/// Sets the held data.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	Task Set(T? data);

	/// <summary>
	/// Get related acquirable type.
	/// </summary>
	/// <typeparam name="TA">the acquirable type.</typeparam>
	/// <param name="path">Path from Type of this acquirable to the needed requireable</param>
	/// <returns>Acquirable if any.</returns>
	// TODO: review this.
	Task<IAcquirable<TA>?> GetAcquirable<TA>(Expression<Func<T, IAcquirable<TA>?>> path) where TA : class;

	/// <summary>
	/// Attempts to save/commit changes.
	/// </summary>
	/// <returns></returns>
	Task<CommitResult> TryCommitChanges(CommitOption commitOption);

	/// <summary>
	/// Tries to solve the conflict by applying a delegate on our and theirs datas.
	/// The result of the delegate is used to update the acquired entity. Typically in-place.
	/// </summary>
	/// <param name="commitOption">Commit options</param>
	/// <param name="solver">Solver delegate</param>
	/// <returns></returns>
	Task<CommitResult> TryResolveConflict(CommitOption commitOption, ConflictSolver<T> solver);
}

/// <summary>
/// Conflict solving delegate.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="ours">Our version of data</param>
/// <param name="theirs"></param>
/// <returns></returns>
public delegate T? ConflictSolver<T>(T? ours, T? theirs);

public enum CommitOption
{
	/// <summary>
	/// Overwrite
	/// </summary>
	Overwrite,
	/// <summary>
	/// Throw on Conflict
	/// </summary>
	ThrowOnConflict,
	/// <summary>
	/// Update data if it has not been changed. Gently fail otherwise.
	/// </summary>
	UpdateIfNotUpdated,
}

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