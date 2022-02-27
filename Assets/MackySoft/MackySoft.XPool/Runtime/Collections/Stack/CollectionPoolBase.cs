using System;
using System.Collections.Generic;
using MackySoft.XPool.Internal;

namespace MackySoft.XPool.Collections.ObjectModel {
	public abstract class CollectionPoolBase<TCollection,TItem> : IPool<TCollection> where TCollection : class {

		const int kMaxPoolSize = 32;

		readonly Stack<TCollection> m_Pool;

		// The new() constraint in generics use Activator.CreateInstance, which incurs overhead during object creation.
		// Therefore, explicit constructor call by factory method are used to avoid the overhead.
		readonly Func<TCollection> m_Factory;

		// Stack<T> and Queue<T> do not implement ICollection<T>, so need to use a delegate to call the Clear method instead.
		readonly Action<TCollection> m_Clear;

		protected CollectionPoolBase (Func<TCollection> factory,Action<TCollection> clear) {
			if (factory == null) {
				throw Error.ArgumentNullException(nameof(factory));
			}
			if (clear == null) {
				throw Error.ArgumentNullException(nameof(clear));
			}
			m_Pool = new Stack<TCollection>(kMaxPoolSize);
			m_Factory = factory;
			m_Clear = clear;
		}

		public TCollection Rent () {
			lock (m_Pool) {
				return (m_Pool.Count == 0) ? m_Factory() : m_Pool.Pop();
			}
		}

		public void Return (TCollection collection) {
			if (collection == null) {
				return;
			}

			m_Clear(collection);

			lock (m_Pool) {
				if (m_Pool.Count < kMaxPoolSize) {
					m_Pool.Push(collection);
				}
			}
		}

		public void ReleaseInstances (int keep) {
			if ((keep < 0) || (keep > kMaxPoolSize)) {
				throw Error.ArgumentOutOfRangeOfCollection(nameof(keep));
			}

			lock (m_Pool) {
				if (keep != 0) {
					for (int i = m_Pool.Count - keep;i > 0;i--) {
						m_Pool.Pop();
					}
				}
				else {
					m_Pool.Clear();
				}
			}
		}
	}
}