using System;
using MackySoft.XPool.Collections.Internal;

namespace MackySoft.XPool.Collections {
	public struct TemporaryList<T> : IDisposable {

		T[] m_Array;
		int m_Count;

		public int Count => m_Count;

		public int Capacity => m_Array.Length;

		public T[] Array => m_Array;

		public T this[int index] {
			get => index >= 0 && index < m_Count ? m_Array[index] : throw new IndexOutOfRangeException();
			set => m_Array[index] = value;
		}

		public TemporaryList (T[] array) {
			m_Array = array;
			m_Count = 0;
		}

		/// <summary>
		/// Set item to current length and increase length.
		/// </summary>
		public void Add (T item) {
			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Count);
			m_Array[m_Count] = item;
			m_Count++;
		}

		public bool RemoveAt (int index) {
			if (index >= m_Count) {
				return false;
			}
			m_Count--;
			for (int i = index;i < m_Count;i++) {
				m_Array[i] = m_Array[i + 1];
			}
			return true;
		}

		public void Clear (bool clearArray = false) {
			ArrayPool<T>.Shared.Return(m_Array,clearArray);

			m_Array = ArrayPool<T>.Shared.Rent(0);
			m_Count = 0;
		}

		public void Dispose () {
			Dispose(!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
		}

		public void Dispose (bool clearArray) {
			ArrayPool<T>.Shared.Return(ref m_Array,clearArray);
			m_Count = 0;
		}

	}

}