namespace Name.Bayfaderix.Darxxemiyur.Common
{
	public static class EnumerableExtensions
	{
		public static int GetSequenceHashCode<TItem>(this IEnumerable<TItem> list)
		{
			if (list == null)
				return 0;
			const int seedValue = 0x2D2816FE;
			const int primeNumber = 397;
			return list.Aggregate(seedValue + list.GetHashCode(), (current, item) => (current * primeNumber) + (Equals(item, default(TItem)) ? 0 : item.GetHashCode()));
		}

		private static async Task<LinkedListNode<Task<T>>> ToMyThing<T>(LinkedListNode<Task<T>> g)
		{
			await g.Value.ConfigureAwait(false);
			return g;
		}

		public static IEnumerable<Task<LinkedListNode<Task<T>>>> ToMyThingy<T>(this LinkedList<Task<T>> values)
		{
			var node = values.First;
			while (node != null)
			{
				yield return ToMyThing(node);
				node = node.Next;
			}
		}

		public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> input)
		{
			var list = new LinkedList<T>();
			foreach (var item in input)
				list.AddLast(item);
			return list;
		}

		public static IEnumerable<TItem> AsSaturatedTape<TItem>(this IEnumerable<TItem> components, Func<int, TItem> onLeft, Func<int, TItem> onRight, int maxPerChunk, Func<int, TItem> filler)
		{
			components = AsMarkedTape(components, onLeft, onRight, maxPerChunk);
			var max = components.Count();
			var chunks = (int)Math.Ceiling((double)max / maxPerChunk) * maxPerChunk - max;
			return components.Concat(Enumerable.Range(1, chunks).Select(x => filler(x)));
		}

		public static IEnumerable<TItem> AsMarkedTape<TItem>(this IEnumerable<TItem> components, Func<int, TItem> onLeft, Func<int, TItem> onRight, int maxPerChunk)
		{
			var max = components.Count();
			var i = 0;
			foreach (var component in components)
			{
				if (i % maxPerChunk == 0 && i > 0)
				{
					i++;
					max++;
					yield return onLeft(i);
				}
				i++;
				yield return component;
				if (i % maxPerChunk == maxPerChunk - 1 && i < max - 1)
				{
					i++;
					max++;
					yield return onRight(i);
				}
			}
		}
	}
}