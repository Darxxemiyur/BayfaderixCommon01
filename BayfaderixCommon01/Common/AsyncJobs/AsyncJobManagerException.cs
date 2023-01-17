using System.Runtime.Serialization;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class AsyncJobManagerException : BayfaderixCommonException
	{
		public override string StackTrace => HideSecretStackTrace(base.StackTrace, x => x.Contains(nameof(AsyncJobManager)) || x.Contains(nameof(AsyncJob)));

		/// <inheritdoc/>
		public AsyncJobManagerException()
		{
		}

		/// <inheritdoc/>
		public AsyncJobManagerException(Exception? innerException) : base(innerException?.Message, innerException)
		{
		}

		/// <inheritdoc/>
		protected AsyncJobManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}