namespace Name.Bayfaderix.Darxxemiyur.General
{
	/// <summary>
	/// Identity interface.
	/// </summary>
	public interface IIdentity
	{
		/// <summary>
		/// Compares two instances for equality.
		/// </summary>
		/// <param name="to"></param>
		/// <returns>
		/// True if they are equal according to the implementation of the interface. False otherwise.
		/// </returns>
		bool Equals(IIdentity to);
	}
}