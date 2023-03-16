using Name.Bayfaderix.Darxxemiyur.Common;

namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	/// <summary>
	/// Node interface that describes what it is capable of.
	/// </summary>
	public interface INode : IAsyncRunnable
	{
		bool IsSink {
			get;
		}

		bool IsSource {
			get;
		}

		bool IsBi {
			get;
		}
	}
}
