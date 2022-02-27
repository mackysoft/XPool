using System.Collections.Generic;
using MackySoft.XPool.Collections.ObjectModel;

namespace MackySoft.XPool.Collections {
	public class ListPool<T> : CollectionPoolBase<List<T>,T> {

		public static readonly ListPool<T> Shared = new ListPool<T>();

		public ListPool () : base(kDefaultCapacity,() => new List<T>(),list => list.Clear()) {
		}
	}

	public class QueuePool<T> : CollectionPoolBase<Queue<T>,T> {

		public static readonly QueuePool<T> Shared = new QueuePool<T>();

		public QueuePool () : base(kDefaultCapacity,() => new Queue<T>(),queue => queue.Clear()) {
		}
	}

	public class StackPool<T> : CollectionPoolBase<Stack<T>,T> {

		public static readonly StackPool<T> Shared = new StackPool<T>();

		public StackPool () : base(kDefaultCapacity,() => new Stack<T>(),stack => stack.Clear()) {
		}
	}

	public class HashSetPool<T> : CollectionPoolBase<HashSet<T>,T> {

		public static readonly HashSetPool<T> Shared = new HashSetPool<T>();

		public HashSetPool () : base(kDefaultCapacity,() => new HashSet<T>(),hashSet => hashSet.Clear()) {
		}
	}
}