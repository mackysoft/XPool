using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.XPool.Collections.Internal;
using MackySoft.XPool.Internal;

namespace MackySoft.XPool.Collections {

	/// <summary>
	/// <para> Temporary queue without allocation. </para>
	/// <para> This struct use <see cref="ArrayPool{T}"/> internally to avoid allocation and can be used just like a normal queue. </para>
	/// <para> After using it, please call the Dispose(). </para>
	/// </summary>
	public partial struct TemporaryQueue<T> : IEnumerable<T>, IDisposable {

		T[] m_Array;
		ArrayPool<T> m_Pool;
		int m_Count;
		int m_First;
		int m_Last;
		int m_Mask;

		public int Count => m_Count;
		public int Capacity => m_Array.Length;
		public T[] Array => m_Array;

		public TemporaryQueue (ArrayPool<T> pool,int minimumCapacity) {
			m_Pool = pool ?? throw new ArgumentNullException(nameof(pool));
			m_Array = pool.Rent(minimumCapacity);
			m_Count = 0;
			m_First = 0;
			m_Last = 0;
			m_Mask = (m_Array.Length == 0) ? 0 : (m_Array.Length - 1);
		}

		public void Enqueue (T item) {
			if (ArrayPoolUtility.EnsureCapacityCircular(ref m_Array,m_Count,m_Count + 1,ref m_First,ref m_Last,m_Pool)) {
				m_Mask = m_Array.Length - 1;
			}
			m_Array[m_Last] = item;
			m_Last = (m_Last + 1) & m_Mask;
			m_Count++;
		}

		public T Dequeue () {
			if (m_Count == 0) {
				throw Error.Empty();
			}
			T removed = m_Array[m_First];
			m_Array[m_First] = default;
			m_First = (m_First + 1) & m_Mask;
			m_Count--;
			return removed;
		}

		public T Peek () {
			if (m_Count == 0) {
				throw Error.Empty();
			}
			return m_Array[m_First];
		}

		public void Clear () {
			if (m_First < m_Last) {
				System.Array.Clear(m_Array,m_First,m_Count);
			}
			else {
				System.Array.Clear(m_Array,m_First,m_Array.Length - m_First);
				System.Array.Clear(m_Array,0,m_Last);
			}
			m_First = 0;
			m_Last = 0;
			m_Count = 0;
			m_Mask = 0;
		}

		public bool Contains (T item) {
			int index = m_First;
			int count = m_Count;

			if (item == null) {
				while (count-- > 0) {
					if (m_Array[index] == null) {
						return true;
					}
					index = (index + 1) & m_Mask;
				}
			}
			else {
				var c = EqualityComparer<T>.Default;
				while (count-- > 0) {
					T e = m_Array[index];
					if ((e != null) && c.Equals(e,item)) {
						return true;
					}
					index = (index + 1) & m_Mask;
				}
			}
			return false;
		}

		public void CopyTo (T[] array,int arrayIndex) {
			if (array == null) {
				throw new ArgumentNullException(nameof(array));
			}
			if (arrayIndex < 0 || arrayIndex > array.Length) {
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			}
			if (array.Length - arrayIndex < m_Count) {
				throw new ArgumentException();
			}
			
			if (m_First < m_Last) {
				System.Array.Copy(m_Array,m_First,array,arrayIndex,m_Count);
			}
			else {
				System.Array.Copy(m_Array,m_First,array,arrayIndex,m_Array.Length - m_First);
				System.Array.Copy(m_Array,0,array,arrayIndex + (m_Array.Length - m_First),m_Last);
			}
		}

		public void Dispose () {
			m_Pool.Return(ref m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
			m_Pool = null;
			m_First = 0;
			m_Last = 0;
			m_Count = 0;
			m_Mask = 0;
		}

		internal T GetElement (int index) {
			return m_Array[(m_First + index) & m_Mask];
		}

		public IEnumerator<T> GetEnumerator () {
			for (int i = 0;m_Count > i;i++) {
				yield return m_Array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();

	}
}