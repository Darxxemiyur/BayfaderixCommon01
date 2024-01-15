using System.Linq.Expressions;

using Name.Bayfaderix.Darxxemiyur.General;
using Name.Bayfaderix.Darxxemiyur.Abstract;

namespace Name.Bayfaderix.Darxxemiyur.DataSource.Abstract;

/// <summary>
/// Presents an <see cref="IIdentifiable{T}"/> entity that can be asynchroniously acquired.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAcquirable<T> where T : class
{
	/// <summary>
	/// Parent repository of this <see cref="IAcquirable{T}"/>
	/// </summary>
	IAcquirablesRepository<T> Repository
	{
		get;
	}

	/// <summary>
	/// Identity of <see cref="IAcquirable{T}"/> entity.
	/// </summary>
	IIdentity? Identity
	{
		get;
	}

	/// <summary>
	/// Ask instance if it knows how to acquire the entity.
	/// </summary>
	/// <returns>Returns true if it knows how to acquire the entity. False otherwise.</returns>
	Task<bool> IsAcquirable();
	bool IsLoaded(T acquired, Expression<Func<T, IIdentifiable<object>>> expression);
	Task<bool> TryLoad(T acquired, Expression<Func<T, IIdentifiable<object>>> expression);
	bool IsLoaded<TI>(Expression<Func<T, IIdentifiable<TI>>> expression);
	Task<bool> TryLoad<TI>(Expression<Func<T, IIdentifiable<TI>>> expression);

	/// <summary>
	/// Attempt to acquire the entity.
	/// </summary>
	/// <returns>Entity if any.</returns>
	Task<IIdentifiable<T>?> Acquire();

	/// <summary>
	/// Sets the held data.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	Task Set(T? data);

	/// <summary>
	/// Sets the held data.
	/// </summary>
	/// <param name="data"></param>
	/// <returns></returns>
	Task Set(IIdentifiable<T>? data);

	/// <summary>
	/// Get related acquirable type.
	/// </summary>
	/// <typeparam name="TA">the acquirable type.</typeparam>
	/// <param name="path">Path from Type of this acquirable to the needed requireable</param>
	/// <returns>Acquirable if any.</returns>
	// TODO: review this.
	Task<IAcquirable<TA>?> GetAcquirable<TA>(Expression<Func<T, IAcquirable<TA>?>> path) where TA : class;

	/// <summary>
	/// Attempts to save/commit changes of related entities.
	/// <br/>
	/// [Note: Does not guarantee saving changes of all entities acquired from the <see cref="Repository"/>]
	/// </summary>
	/// <returns></returns>
	Task<CommitResult> TryCommitChanges(CommitOption commitOption);

	/// <summary>
	/// Tries to solve the conflicting entities by applying a delegate on our and theirs datas.
	/// The result of the delegate is used to update the acquired entity. Typically in-place.
	/// <br/>
	/// [Same note as in <see cref="TryCommitChanges(CommitOption)"/>]
	/// </summary>
	/// <param name="commitOption">Commit options</param>
	/// <param name="solver">Solver delegate</param>
	/// <returns></returns>
	Task<CommitResult> TryResolveConflict(CommitOption commitOption, ConflictSolver<T> solver);
}

