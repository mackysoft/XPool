using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.XPool.Collections.Internal;

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

		public bool Contains (T item) {
			if (item == null) {
				for (int i = 0;i < m_Count;i++) {
					if (item == null) {
						return true;
					}
				}
				return false;
			}
			else {
				var comparer = EqualityComparer<T>.Default;
				for (int i = 0;i < m_Count;i++) {
					if (comparer.Equals(m_Array[i],item)) {
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Set item to current length and increase length.
		/// </summary>
		public void Add (T item) {
			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Count,m_Pool);
			m_Array[m_Count] = item;
			m_Count++;
		}

		public void AddRange (IEnumerable<T> collection) {
			InsertRange(m_Count,collection);
		}

		public void Insert (int index,T item) {
			if (index > m_Count) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Count + 1,m_Pool);

			if (index < m_Count) {
				System.Array.Copy(m_Array,index,m_Array,index + 1,m_Count - index);
			}

			m_Array[index] = item;
			m_Count++;
		}

		public void InsertRange (int index,IEnumerable<T> collection) {
			if (collection is ICollection<T> c) {
				int count = c.Count;

				ArrayPoolUtility.EnsureCapacity(ref m_Array,m_Count + count,m_Pool);
				if (index < m_Count) {
					System.Array.Copy(m_Array,index,m_Array,index + count,m_Count - index);
				}

				c.CopyTo(m_Array,index);

				m_Count += count;
			}
			else {
				foreach (T item in collection) {
					Insert(index,item);
					index++;
				}
			}
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
			if (index < m_Count) {
				System.Array.Copy(m_Array,index + 1,m_Array,index,m_Count - index);
			}
			m_Array[m_Count] = default;
			return true;
		}

		public void RemoveRange (int index,int count) {
			if (index < 0) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			if (m_Count - index < count) {
				throw new ArgumentException();
			}

			if (count > 0) {
				m_Count -= count;
				if (index < m_Count) {
					System.Array.Copy(m_Array,index + count,m_Array,index,m_Count - index);;
				}
				System.Array.Clear(m_Array,m_Count,count);
			}
		}

		public int RemoveAll (Predicate<T> match) {
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}

			int freeIndex = 0;
			while (freeIndex < m_Count && !match(m_Array[freeIndex])) {
				freeIndex++;
			}
			if (freeIndex >= m_Count) {
				return 0;
			}

			int current = freeIndex + 1;
			while (current  < m_Count) {
				while (current < m_Count && match(m_Array[current])) {
					current++;
				}
				if (current < m_Count) {
					m_Array[freeIndex++] = m_Array[current++];
				}
			}

			System.Array.Clear(m_Array,freeIndex,m_Count - freeIndex);
			int result = m_Count - freeIndex;
			m_Count = freeIndex;
			return result;
		}

		public void Clear () {
			m_Pool.Return(m_Array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());

			m_Array = m_Pool.Rent(0);
			m_Count = 0;
		}

		#region IndexOf

		public int IndexOf (T item) {
			return System.Array.IndexOf(m_Array,item,0,m_Count);
		}

		public int IndexOf (T item,int index) {
			if (index > m_Count) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			return System.Array.IndexOf(m_Array,item,index,m_Count - index);
		}

		public int IndexOf (T item,int index,int count) {
			if (index > m_Count) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			if (count < 0 || index > m_Count - count) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			return System.Array.IndexOf(m_Array,item,index,count);
		}

		#endregion

		#region LastIndexOf

		public int LastIndexOf (T item) {
			if (m_Count == 0) {
				return -1;
			}
			return LastIndexOf(item,m_Count - 1,m_Count);
		}

		public int LastIndexOf (T item,int index) {
			if (index >= m_Count) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			return LastIndexOf(item,index,index + 1);
		}

		public int LastIndexOf (T item,int index,int count) {
			if (m_Count == 0) {
				if (index < 0) {
					throw new ArgumentOutOfRangeException(nameof(index));
				}
				if (count < 0) {
					throw new ArgumentOutOfRangeException(nameof(count));
				}
				return -1;
			}
			if (index >= m_Count) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			if (m_Count > index + 1) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			return System.Array.LastIndexOf(m_Array,item,index,count);
		}

		#endregion

		#region BinarySearch

		public int BinarySearch (T item) {
			return BinarySearch(0,m_Count,item,null);
		}

		public int BinarySearch (T item,IComparer<T> comparer) {
			return BinarySearch(0,m_Count,item,comparer);
		}

		public int BinarySearch (int index,int count,T item,IComparer<T> comparer) {
			if (index < 0) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			if (m_Count - index < count) {
				throw new ArgumentException();
			}
			return System.Array.BinarySearch(m_Array,index,count,item,comparer);
		}

		#endregion

		#region CopyTo

		public void CopyTo (T[] array) {
			CopyTo(array,0);
		}

		public void CopyTo (T[] array,int arrayIndex) {
			System.Array.Copy(m_Array,0,array,arrayIndex,m_Count);
		}

		public void CopyTo (int index,T[] array,int arrayIndex,int count) {
			if (m_Count - index < count) {
				throw new ArgumentException();
			}
			System.Array.Copy(m_Array,index,array,arrayIndex,count);
		}

		#endregion

		#region Reverse

		public void Reverse () {
			Reverse(0,m_Count);
		}

		public void Reverse (int index,int count) {
			if (index < 0) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			if (m_Count - index < count) {
				throw new ArgumentException();
			}
			System.Array.Reverse(m_Array,index,count);
		}

		#endregion

		#region Sort

		public void Sort () {
			Sort(0,m_Count,null);
		}

		public void Sort (IComparer<T> comparer) {
			Sort(0,m_Count,comparer);
		}

		public void Sort (int index,int count,IComparer<T> comparer) {
			if (index < 0) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			if (m_Count - index < count) {
				throw new ArgumentException();
			}

			System.Array.Sort(m_Array,index,count,comparer);
		}

		#endregion

		#region FindIndex

		public int FindIndex (Predicate<T> match) {
			return FindIndex(0,match);
		}

		public int FindIndex (int startIndex,Predicate<T> match) {
			return FindIndex(startIndex,m_Count,match);
		}

		public int FindIndex (int startIndex,int count,Predicate<T> match) {
			if (startIndex > m_Count) {
				throw new ArgumentOutOfRangeException(nameof(startIndex));
			}
			if (count < 0 || startIndex > m_Count - count) {
				throw new ArgumentException();
			}
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}

			int endIndex = startIndex + count;
			for (int i = startIndex;i < endIndex;i++) {
				if (match(m_Array[i])) {
					return i;
				}
			}
			return -1;
		}

		#endregion

		#region FindLastIndex

		public int FindLastIndex (Predicate<T> match) {
			return FindLastIndex(m_Count - 1,m_Count,match);
		}

		public int FindLastIndex (int startIndex,Predicate<T> match) {
			return FindLastIndex(startIndex,startIndex + 1,match);
		}

		public int FindLastIndex (int startIndex,int count,Predicate<T> match) {
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}
			if (m_Count == 0) {
				if (startIndex == -1) {
					throw new ArgumentOutOfRangeException(nameof(startIndex));
				}
			}
			else {
				if (startIndex >= m_Count) {
					throw new ArgumentOutOfRangeException(nameof(startIndex));
				}
			}
			if (count < 0 || startIndex - count + 1 < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			int endIndex = startIndex - count;
			for (int i = startIndex;i > endIndex;i--) {
				if (match(m_Array[i])) {
					return i;
				}
			}
			return -1;
		}

		#endregion

		public bool Exists (Predicate<T> match) {
			return FindIndex(match) != -1;
		}

		public T Find (Predicate<T> match) {
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}

			for (int i = 0;i < m_Count;i++) {
				T item = m_Array[i];
				if (match(item)) {
					return item;
				}
			}
			return default;
		}

		public T FindLast (Predicate<T> match) {
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}

			for (int i = m_Count - 1;i >= 0;i--) {
				T item = m_Array[i];
				if (match(item)) {
					return item;
				}
			}
			return default;
		}

		public TemporaryList<T> FindAll (Predicate<T> match) {
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}

			TemporaryList<T> result = Create(m_Pool);
			for (int i = 0;i < m_Count;i++) {
				T item = m_Array[i];
				if (match(item)) {
					result.Add(item);
				}
			}
			return result;
		}

		public bool TrueForAll (Predicate<T> match) {
			if (match == null) {
				throw new ArgumentNullException(nameof(match));
			}

			for (int i = 0;i < m_Count;i++) {
				if (!match(m_Array[i])) {
					return false;
				}
			}
			return true;
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