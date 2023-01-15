using System.Runtime.Serialization;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class BayfaderixCommonException : Exception
	{
		protected static string HideSecretStackTrace(string stackTrace, Func<string, bool> toHide) => //San(
			string.Join(Environment.NewLine, (stackTrace ?? "").Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
			//.Select(x => toHide(x) ? $"{x}\n^^^Will Hide!^^^" : x)
			.Where(x => !x.Contains("--- End of stack trace from previous location ---") && !x.Contains(typeof(BayfaderixCommonException).Namespace) && !x.Contains($"{nameof(BayfaderixCommonException)}.{nameof(HideSecretStackTrace)}") && !toHide(x))
			);

		//, "^^^Will Hide!^^^");

		private static string San(string inp, string kill) => inp.Replace($"{kill}\n{kill}", kill, StringComparison.OrdinalIgnoreCase).Trim() is var n && n == inp ? inp : San(n, kill);

		public override string StackTrace => HideSecretStackTrace(string.Join("\n", SafeStack, base.StackTrace), x => false);
		private readonly string SafeStack;

		/// <inheritdoc/>
		public BayfaderixCommonException()
		{
		}

		/// <inheritdoc/>
		public BayfaderixCommonException(string? message) : base(message)
		{
		}

		/// <inheritdoc/>
		public BayfaderixCommonException(string? message, Exception? innerException) : base(message, Check(innerException) ? innerException.InnerException : innerException) => SafeStack = Check(innerException) ? innerException.StackTrace : null;

		private static bool Check(Exception? innerException) => (innerException?.GetType()?.IsAssignableTo(typeof(BayfaderixCommonException)) ?? false) && innerException.InnerException != null;

		/// <inheritdoc/>
		//public BayfaderixCommonException(string? message, Exception? innerException) : base(message,  innerException) => SafeStack =  "";

		/// <inheritdoc/>
		protected BayfaderixCommonException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}