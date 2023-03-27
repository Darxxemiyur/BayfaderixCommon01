namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// TellInternally's result interface.
/// </summary>
public interface ITellResult
{
	/// <summary>
	/// Result code.
	/// </summary>
	int Code {
		get;
	}

	/// <summary>
	/// Custom message note.
	/// </summary>
	string? Note {
		get;
	}

	/// <summary>
	/// The message custom result.
	/// </summary>
	object? Result {
		get;
	}
}
