using System;
using MackySoft.XPool.Collections.Internal;

namespace MackySoft.XPool.Collections {

	/// <summary>
	/// <para> Temporary queue without allocation. </para>
	/// <para> This struct use <see cref="ArrayPool{T}"/> internally to avoid allocation and can be used just like a normal queue. </para>
	/// <para> After using it, please call the Dispose(). </para>
	/// </summary>
	public struct TemporaryQueue<T> : IDisposable {

		T[] m_Array;
		ArrayPool<T> m_Pool;
		int m_Count;
		int m_First;
		int m_Last;
		int m_Mask;

		public TemporaryQueue (ArrayPool<T> pool,int minimumCapacity) {
			m_Pool = pool;
			m_Array = pool.Rent(minimumCapacity);
			m_Count = 0;
			m_First = 0;
			m_Last = 0;
			m_Mask = 0;
		}

		public void Enqueue (T item) {
			if (ArrayPoolUtility.EnsureCapacityCircular(ref m_Array,m_Count + 1,ref m_First,ref m_Last,m_Pool)) {
				m_Mask = m_Array.Length - 1;
			}

			m_Array[m_Last] = item;
			m_Last = (m_Last + 1) & m_Mask;
			m_Count++;
		}

		public T Dequeue () {
			if (m_Count == 0) {
				throw new InvalidOperationException();
			}
			T removed = m_Array[m_First];
			m_Array[m_First] = default;
			m_First = (m_First + 1) & m_Mask;
			m_Count--;
			return removed;
		}

		public T Peek () {
			if (m_Count == 0) {
				throw new InvalidOperationException();
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
		}

		public void Dispose () {
			m_Pool.Return(ref m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
			m_First = 0;
			m_Last = 0;
			m_Count = 0;
		}
	}
}