namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// TellInternally's message interface
/// </summary>
public interface ITellMessage
{
	/// <summary>
	/// Custom message note.
	/// </summary>
	string? Note {
		get;
	}

	/// <summary>
	/// The message custom message.
	/// </summary>
	IIdentifiable<object?>? Message {
		get;
	}
}
