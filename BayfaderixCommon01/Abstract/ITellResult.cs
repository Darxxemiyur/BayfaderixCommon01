namespace Name.Bayfaderix.Darxxemiyur.Abstract;

/// <summary>
/// TellInternally's result interface.
/// </summary>
public interface ITellResult<out TObject>
{
	/// <summary>
	/// Result code.
	/// </summary>
	int Code
	{
		get;
	}

	/// <summary>
	/// Custom message note.
	/// </summary>
	string? Note
	{
		get;
	}

	/// <summary>
	/// The message custom result.
	/// </summary>
	IIdentifiable<TObject>? Result
	{
		get;
	}
}
