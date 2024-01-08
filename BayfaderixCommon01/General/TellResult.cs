namespace Name.Bayfaderix.Darxxemiyur.General;

/// <summary>
/// A non nullable value structure. Do not mistake for ITellResult. They are not the same.
/// </summary>
public record struct TellResult<TObject>(ITellResult<TObject>? OriginalResult) : ITellResult<TObject>
{
	/// <summary>
	/// True if TellResult encloses a null. False otherwise.
	/// </summary>
	public bool IsNull => OriginalResult == null;
	/// <summary>
	/// Result code.
	/// </summary>
	public int Code => OriginalResult?.Code ?? -1;
	/// <summary>
	/// Custom message note.
	/// </summary>
	public string? Note => OriginalResult?.Note;
	/// <summary>
	/// The message custom result.
	/// </summary>
	public IIdentifiable<TObject>? Result => OriginalResult?.Result;
}
