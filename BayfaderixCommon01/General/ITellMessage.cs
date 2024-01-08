namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// TellInternally's message interface
/// </summary>
public interface ITellMessage<out TObject>
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
	IIdentifiable<TObject>? Message {
		get;
	}
}
