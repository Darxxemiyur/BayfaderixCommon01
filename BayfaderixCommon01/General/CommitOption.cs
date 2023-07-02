namespace Name.Bayfaderix.Darxxemiyur.General;

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
