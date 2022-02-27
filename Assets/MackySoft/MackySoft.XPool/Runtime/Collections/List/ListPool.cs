using System.Collections.Generic;
using MackySoft.XPool.Collections.ObjectModel;

namespace MackySoft.XPool.Collections {
	public class ListPool<T> : CollectionPoolBase<List<T>,T> {

		public static readonly ListPool<T> Shared = new ListPool<T>();

		public ListPool () : base(() => new List<T>(),list => list.Clear()) {
		}
	}
}