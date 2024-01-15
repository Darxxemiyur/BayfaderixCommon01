using System.Collections;

namespace Name.Bayfaderix.Darxxemiyur.Collections;

public static class CollectionExtensions
{
	public static int GetSequenceHashCode(this IEnumerable list)
	{
		if (list == null)
			return 0;
		const int seedValue = 0x2D2816FE;
		const int primeNumber = 397;
		int value = seedValue + list.GetHashCode();
		foreach (var item in list)
			value += (value * primeNumber) + (item is IEnumerable seq ? GetSequenceHashCode(seq) : item?.GetHashCode() ?? 0);
		return value;
	}

	public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this IAsyncEnumerable<T> enumerable, CancellationToken token = default)
	{
		var list = new LinkedList<T>();
		await foreach (var item in enumerable.WithCancellation(token))
			list.AddLast(item);

		return list;
	}

	public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> input)
	{
		var list = new LinkedList<T>();
		foreach (var item in input)
			list.AddLast(item);
		return list;
	}
}
