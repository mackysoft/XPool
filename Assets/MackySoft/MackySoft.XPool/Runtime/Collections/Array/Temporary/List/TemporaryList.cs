using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.XPool.Collections.Internal;
using UnityEngine;

namespace MackySoft.XPool.Collections {

	/// <summary>
	/// <para> Temporary list without allocation. </para>
	/// <para> This struct use <see cref="ArrayPool{T}"/> internally to avoid allocation and can be used just like a normal list. </para>
	/// <para> After using it, please call the Dispose(). </para>
	/// </summary>
	public partial struct TemporaryList<T> : IEnumerable<T>, IDisposable {

		T[] m_Array;
		int m_Count;
		ArrayPool<T> m_Pool;

		public int Count => m_Count;

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
			get {
				return index >= 0 && index < m_Count ? m_Array[index] : throw new ArgumentOutOfRangeException(nameof(index));
			}
			set {
				if (index < 0 && index >= m_Count) {
					throw new ArgumentOutOfRangeException(nameof(index));
				}
				m_Array[index] = value;
			}
		}

		public TemporaryList (ArrayPool<T> pool,int minimumCapacity) {
			m_Pool = pool;
			m_Array = pool.Rent(minimumCapacity);
			m_Count = 0;
		}

		internal TemporaryList (ArrayPool<T> pool,T[] array,int count) {
			m_Pool = pool;
			m_Array = array;
			m_Count = count;
		}

		/// <summary>
		/// Set item to current length and increase length.
		/// </summary>
		public void Add (T item) {
			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Count,m_Pool);
			m_Array[m_Count] = item;
			m_Count++;
		}

		public bool Remove (T item) {
			int index = IndexOf(item);
			if (index >= 0) {
				RemoveAt(index);
				return true;
			}
			return false;
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

		public int IndexOf (T item) {
			return System.Array.IndexOf(m_Array,item,0,m_Count);
		}

		public void Clear () {
			m_Pool.Return(m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());

			m_Array = m_Pool.Rent(0);
			m_Count = 0;
		}

		/// <summary>
		/// Return the internal array to the pool.
		/// </summary>
		public void Dispose () {
			Dispose(!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());
		}

		/// <summary>
		/// Return the internal array to the pool.
		/// </summary>
		public void Dispose (bool clearArray) {
			m_Pool.Return(ref m_Array,clearArray);
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