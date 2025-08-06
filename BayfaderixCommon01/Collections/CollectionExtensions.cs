using System.Collections;
using System.Runtime.CompilerServices;

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
	public static LinkedList<T> ToLinkedList<T>(this IEnumerable<LinkedListNode<T>> input)
	{
		var list = new LinkedList<T>();
		foreach (var item in input)
			list.AddLast(item);
		return list;
	}
	public static async IAsyncEnumerable<T2> Select<T1, T2>(this IAsyncEnumerable<T1> enumerable, Func<T1, Task<T2>> selector, [EnumeratorCancellation] CancellationToken ct = default)
	{
		await foreach (var item in enumerable)
			yield return await selector(item);
	}
	public static async IAsyncEnumerable<T2> Select<T1, T2>(this ConfiguredCancelableAsyncEnumerable<T1> enumerable, Func<T1, Task<T2>> selector, [EnumeratorCancellation] CancellationToken ct = default)
	{
		await foreach (var item in enumerable)
			yield return await selector(item);
	}
	public static async IAsyncEnumerable<T2> SelectMany<T1, T2>(this IAsyncEnumerable<T1> enumerable, Func<T1, IAsyncEnumerable<T2>> selector, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var runners = new LinkedList<Task<(IAsyncEnumerator<T2>, bool, object)>>();

		await foreach (var cep in enumerable)
		{
			if (ct.IsCancellationRequested)
				yield break;

			var epae = selector(cep).GetAsyncEnumerator();
			var node = new LinkedListNode<Task<(IAsyncEnumerator<T2>, bool, object)>>(null!);
			node.Value = epae.MoveNextAsync().AsTask().ContinueWith(x => (epae, x.Result, (object)node));
			runners.AddLast(node);
		}

		await foreach (var cep in Feeder(runners, ct))
			yield return cep;
	}
	public static async IAsyncEnumerable<T2> SelectMany<T1, T2>(this ConfiguredCancelableAsyncEnumerable<T1> enumerable, Func<T1, IAsyncEnumerable<T2>> selector, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var runners = new LinkedList<Task<(IAsyncEnumerator<T2>, bool, object)>>();

		await foreach (var cep in enumerable)
		{
			if (ct.IsCancellationRequested)
				yield break;

			var epae = selector(cep).GetAsyncEnumerator();
			var node = new LinkedListNode<Task<(IAsyncEnumerator<T2>, bool, object)>>(null!);
			node.Value = epae.MoveNextAsync().AsTask().ContinueWith(x => (epae, x.Result, (object)node));
			runners.AddLast(node);
		}

		await foreach (var cep in Feeder(runners, ct))
			yield return cep;
	}
#pragma warning disable CS8425 // Async-iterator member has one or more parameters of type 'CancellationToken' but none of them is decorated with the 'EnumeratorCancellation' attribute, so the cancellation token parameter from the generated 'IAsyncEnumerable<>.GetAsyncEnumerator' will be unconsumed
//Really? Maybe that's because it's not an extension method?
    private static async IAsyncEnumerable<T1> Feeder<T1>(LinkedList<Task<(IAsyncEnumerator<T1>, bool, object)>> runners, CancellationToken ct)
#pragma warning restore CS8425 // Async-iterator member has one or more parameters of type 'CancellationToken' but none of them is decorated with the 'EnumeratorCancellation' attribute, so the cancellation token parameter from the generated 'IAsyncEnumerable<>.GetAsyncEnumerator' will be unconsumed
    {
		while (!ct.IsCancellationRequested && runners.Count > 0)
		{
			var (enumer, advanced, indexT) = await await Task.WhenAny(runners);
			var index = (LinkedListNode<Task<(IAsyncEnumerator<T1>, bool, object)>>)indexT;

			yield return enumer.Current;

			if (!advanced)
			{
				index.List!.Remove(index);
				continue;
			}

			index.Value = enumer.MoveNextAsync().AsTask().ContinueWith(x => (enumer, x.Result, indexT));
		}
	}
}
