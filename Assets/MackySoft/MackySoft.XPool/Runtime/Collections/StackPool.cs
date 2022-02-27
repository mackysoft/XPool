using System.Collections.Generic;
using MackySoft.XPool.Collections.ObjectModel;

namespace MackySoft.XPool.Collections {
	public class StackPool<T> : CollectionPoolBase<Stack<T>,T> {

		public static readonly StackPool<T> Shared = new StackPool<T>();

		public StackPool () : base(() => new Stack<T>(),stack => stack.Clear()) {
		}
	}
}