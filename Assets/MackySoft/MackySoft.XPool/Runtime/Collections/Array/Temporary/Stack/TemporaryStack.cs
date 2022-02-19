using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.XPool.Collections.Internal;

namespace MackySoft.XPool.Collections {
	public partial struct TemporaryStack<T> : IEnumerable<T>, IDisposable {

		T[] m_Array;
		ArrayPool<T> m_Pool;
		int m_Count;

		public int Count => m_Count;

		public TemporaryStack (ArrayPool<T> pool,int minimumCapacity) {
			if (pool == null) {
				throw new ArgumentNullException(nameof(pool));
			}
			m_Pool = pool;
			m_Array = pool.Rent(minimumCapacity);
			m_Count = 0;
		}

		public void Push (T item) {
			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Count + 1,m_Pool);
			m_Array[m_Count] = item;
			m_Count++;
		}

		public T Pop () {
			if (m_Count == 0) {
				throw new InvalidOperationException();
			}
			T item = m_Array[m_Count - 1];
			m_Array[m_Count - 1] = default;
			m_Count--;
			return item;
		}

		public T Peek () {
			if (m_Count == 0) {
				throw new InvalidOperationException();
			}
			return m_Array[m_Count - 1];
		}

		public void Dispose () {
			m_Pool.Return(ref m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
			m_Pool = null;
			m_Count = 0;
		}

		public IEnumerator<T> GetEnumerator () {
			for (int i = 0;i < m_Count;i++) {
				yield return m_Array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

	}
}