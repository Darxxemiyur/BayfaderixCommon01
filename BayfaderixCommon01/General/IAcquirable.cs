using System.Linq.Expressions;

namespace Name.Bayfaderix.Darxxemiyur.General
{
	/// <summary>
	/// Presents an entity that can be asynchroniously acquired.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IAcquirable<T> where T : class
	{
		/// <summary>
		/// Primarily intended to be used with optional configuring.
		/// </summary>
		IMessageCommunicable Communicable => new StupidMessageCommunicable();

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
		/// Get related acquirable type.
		/// </summary>
		/// <typeparam name="TA">the acquirable type.</typeparam>
		/// <param name="path">Path from Type of this acquirable to the needed requireable</param>
		/// <returns>Acquirable if any.</returns>
		// TODO: review this.
		Task<IAcquirable<TA>?> GetAcquirable<TA>(Expression<Func<T, IAcquirable<TA>?>> path) where TA : class;
	}
}
