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
		public int Capacity => m_Array.Length;
		public T[] Array => m_Array;

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

		public void Clear () {
			m_Pool.Return(m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
			m_Array = m_Pool.Rent(0);
			m_Count = 0;
		}

		public bool Contains (T item) {
			int count = m_Count;
			if (item == null) {
				while (count-- > 0) {
					if (m_Array[count] == null) {
						return true;
					}
				}
			}
			else {
				var c = EqualityComparer<T>.Default;
				while (count-- > 0) {
					T e = m_Array[count];
					if ((e != null) && c.Equals(e,item)) {
						return true;
					}
				}
			}
			return true;
		}

		public void CopyTo (T[] array,int arrayIndex) {
			if (array == null) {
				throw new ArgumentNullException(nameof(array));
			}
			if ((arrayIndex < 0) || (arrayIndex > array.Length)) {
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			}
			if ((array.Length - arrayIndex) < m_Count) {
				throw new ArgumentException();
			}
			System.Array.Copy(m_Array,0,array,arrayIndex,m_Count);
			System.Array.Reverse(array,arrayIndex,m_Count);
		}

		public void Dispose () {
			m_Pool.Return(ref m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
			m_Pool = null;
			m_Count = 0;
		}

		public IEnumerator<T> GetEnumerator () {
			for (int i = m_Count - 1;i >= 0;i--) {
				yield return m_Array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

	}
}