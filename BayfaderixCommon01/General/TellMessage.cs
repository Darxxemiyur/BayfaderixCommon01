using Name.Bayfaderix.Darxxemiyur.Abstract;

namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// A non nullable value structure. Do not mistake for ITellMessage. They are not the same.
/// </summary>
public record struct TellMessage<TObject>(ITellMessage<TObject>? OriginalMessage) : ITellMessage<TObject>
{
	/// <summary>
	/// True if TellMessage encloses a null. False otherwise.
	/// </summary>
	public bool IsNull => OriginalMessage == null;
	/// <summary>
	/// Custom message note.
	/// </summary>
	public string? Note => OriginalMessage?.Note;
	/// <summary>
	/// The message custom message.
	/// </summary>
	public IIdentifiable<TObject>? Message => OriginalMessage?.Message;
}
