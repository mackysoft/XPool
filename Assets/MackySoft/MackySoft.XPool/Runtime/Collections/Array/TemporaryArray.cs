using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.XPool.Collections.Internal;

namespace MackySoft.XPool.Collections {

	/// <summary>
	/// <para> Temporary array without allocation. </para>
	/// <para> This struct use <see cref="ArrayPool{T}"/> internally to avoid allocation and can be used just like a normal array. </para>
	/// <para> After using it, please call the Dispose(). </para>
	/// </summary>
	public partial struct TemporaryArray<T> : IEnumerable<T>, IDisposable {

		T[] m_Array;
		int m_Length;

		public int Length => m_Length;

		/// <summary>
		/// Length of internal array.
		/// </summary>
		public int Capacity => m_Array.Length;

		/// <summary>
		/// <para> Internal array. </para>
		/// <para> The length of internal array is always greater than or equal to <see cref="Length"/> property. </para>
		/// </summary>
		public T[] Array => m_Array;

		public T this[int index] {
			get => index >= 0 && index < m_Length ? m_Array[index] : throw new IndexOutOfRangeException();
			set => m_Array[index] = value;
		}

		public TemporaryArray (T[] array,int length) {
			m_Array = array;
			m_Length = length;
		}

		/// <summary>
		/// Set item to current length and increase length.
		/// </summary>
		public void Add (T item) {
			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Length);
			m_Array[m_Length] = item;
			m_Length++;
		}

		public bool RemoveAt (int index) {
			if (index >= m_Length) {
				return false;
			}
			m_Length--;
			for (int i = index;i < m_Length;i++) {
				m_Array[i] = m_Array[i + 1];
			}
			return true;
		}

		public void Clear (bool clearArray = false) {
			ArrayPool<T>.Shared.Return(m_Array,clearArray);

			m_Array = ArrayPool<T>.Shared.Rent(0);
			m_Length = 0;
		}

		public void Dispose () {
			Dispose(!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
		}

		public void Dispose (bool clearArray) {
			ArrayPool<T>.Shared.Return(ref m_Array,clearArray);
			m_Length = 0;
		}

		public IEnumerator<T> GetEnumerator () {
			for (int i = 0;m_Length > i;i++) {
				yield return m_Array[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return GetEnumerator();
		}
	}
	
}