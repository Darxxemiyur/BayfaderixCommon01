using System.Collections;
using System.Linq.Expressions;

namespace Name.Bayfaderix.Darxxemiyur.General;

public interface IAcquirablesRepository<T> : IQueryable<IAcquirable<T>>, IQueryable, IEnumerable<IAcquirable<T>>, IEnumerable, IAsyncEnumerable<IAcquirable<T>> where T : class
{
	/// <summary>
	/// Primarily intended to be used with optional configuring.
	/// </summary>
	IMessageCommunicable? QnA => new StupidMessageCommunicable();

	/// <summary>
	/// Asynchroniously fetches IAcquirable implementation
	/// </summary>
	/// <param name="predicate"></param>
	/// <returns></returns>
	Task<IAcquirable<T>> GetAcquirable(Expression<Func<T, bool>>? predicate);

	IAsyncEnumerable<IAcquirable<T>> GetAcquirables(Expression<Func<T, bool>>? predicate);

	bool IsLoaded(T acquired, Expression<Func<T, IIdentifiable<object>>> expression);
	Task<bool> TryLoad(T acquired, Expression<Func<T, IIdentifiable<object>>> expression);
	bool IsLoaded<TI>(Expression<Func<T, IIdentifiable<TI>>> expression);
	Task<bool> TryLoad<TI>(Expression<Func<T, IIdentifiable<TI>>> expression);

	/// <summary>
	/// Attempts to save/commit all changes.
	/// </summary>
	/// <returns></returns>
	Task<CommitResult> TryCommitChanges(CommitOption commitOption);

	/// <summary>
	/// Tries to solve the conflict by applying a delegate on our and theirs datas.
	/// The result of the delegate is used to update the acquired entities. Typically in-place.
	/// Applies the same solver to every conflict.
	/// </summary>
	/// <param name="commitOption">Commit options</param>
	/// <param name="solver">Solver delegate</param>
	/// <returns></returns>
	Task<CommitResult> TryResolveConflict(CommitOption commitOption, ConflictSolver<T> solver);

}
