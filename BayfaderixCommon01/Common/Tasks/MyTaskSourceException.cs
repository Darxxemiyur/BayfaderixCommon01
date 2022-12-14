using System.Runtime.Serialization;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class MyTaskSourceException : BayfaderixCommonException
	{
		public override string StackTrace => HideSecretStackTrace(base.StackTrace, x => x.Contains(nameof(MyTaskSource)));

		/// <inheritdoc/>
		public MyTaskSourceException()
		{
		}

		/// <inheritdoc/>
		public MyTaskSourceException(Exception? innerException) : base("", innerException)
		{
		}

		/// <inheritdoc/>
		protected MyTaskSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}