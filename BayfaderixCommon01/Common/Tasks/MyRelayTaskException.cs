using System.Runtime.Serialization;

namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public class MyRelayTaskException : BayfaderixCommonException
	{
		public override string StackTrace => HideSecretStackTrace(base.StackTrace, x => x.Contains(nameof(MyRelayTask)) || x.Contains(nameof(ExtensionsForMyRelayTask)));

		/// <inheritdoc/>
		public MyRelayTaskException()
		{
		}

		/// <inheritdoc/>
		public MyRelayTaskException(Exception? innerException) : base("", innerException)
		{
		}

		/// <inheritdoc/>
		protected MyRelayTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}