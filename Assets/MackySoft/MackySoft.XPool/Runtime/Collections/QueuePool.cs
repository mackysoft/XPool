using System.Collections.Generic;
using MackySoft.XPool.Collections.ObjectModel;

namespace MackySoft.XPool.Collections {
	public class QueuePool<T> : CollectionPoolBase<Queue<T>,T> {

		public static readonly QueuePool<T> Shared = new QueuePool<T>();

		public QueuePool () : base(() => new Queue<T>(),queue => queue.Clear()) {
		}
	}
}