namespace Name.Bayfaderix.Darxxemiyur.Node.Linkable
{
	/// <summary> Type to describe input for a node that will result in giving different
	/// DescriptioningData<T> depending on this type. </summary> <typeparam name="T">Contained argument</typeparam>
	public class DescriptioningInput<T>
	{
		public T Data {
			get;
		}

		public DescriptioningInput(T data) => Data = data;
	}
}
