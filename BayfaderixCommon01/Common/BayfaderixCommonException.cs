using System.Runtime.Serialization;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class BayfaderixCommonException : Exception
	{
		protected static string HideSecretStackTrace(string stackTrace, Func<string, bool> toHide) => //San(
			string.Join(Environment.NewLine, (stackTrace ?? "").Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
			//.Select(x => toHide(x) ? $"{x}\n^^^Will Hide!^^^" : x)
			.Where(x => true || !(toHide(x) && !x.Contains($"{nameof(BayfaderixCommonException)}.{nameof(HideSecretStackTrace)}")))
			);

		//, "^^^Will Hide!^^^");

		private static string San(string inp, string kill) => inp.Replace($"{kill}\n{kill}", kill, StringComparison.OrdinalIgnoreCase).Trim() is var n && n == inp ? inp : San(n, kill);

		public override string StackTrace => HideSecretStackTrace(string.Join("\n", SafeStack, base.StackTrace), x => false);
		private string SafeStack = "";

		/// <inheritdoc/>
		public BayfaderixCommonException()
		{
		}

		/// <inheritdoc/>
		public BayfaderixCommonException(string? message) : base(message)
		{
		}

		/// <inheritdoc/>
		public BayfaderixCommonException(string? message, Exception? innerException) : base(message, (innerException?.GetType()?.IsAssignableTo(typeof(BayfaderixCommonException)) ?? false) && innerException.InnerException != null ? innerException.InnerException : innerException) => SafeStack = innerException?.StackTrace ?? "";

		/// <inheritdoc/>
		protected BayfaderixCommonException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}