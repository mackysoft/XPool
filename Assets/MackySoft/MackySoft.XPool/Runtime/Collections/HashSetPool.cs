using System.Collections.Generic;
using MackySoft.XPool.Collections.ObjectModel;

namespace MackySoft.XPool.Collections {
	public class HashSetPool<T> : CollectionPoolBase<HashSet<T>,T> {

		public static readonly HashSetPool<T> Shared = new HashSetPool<T>();

		public HashSetPool () : base(() => new HashSet<T>(),hashSet => hashSet.Clear()) {
		}
	}
}