using System.Runtime.Serialization;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class AsyncJobException : BayfaderixCommonException
	{
		public override string StackTrace => HideSecretStackTrace(base.StackTrace, x => x.Contains(nameof(AsyncJobManager)) || x.Contains(nameof(AsyncJob)));

		/// <inheritdoc/>
		public AsyncJobException()
		{
		}

		/// <inheritdoc/>
		public AsyncJobException(Exception? innerException) : base(innerException?.Message, innerException)
		{
		}

		/// <inheritdoc/>
		protected AsyncJobException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}