using System;
using System.Collections.Generic;

namespace MackySoft.XPool.Collections {
	public class ListPool<T> {

		const int kMaxBucketSize = 32;

		public static readonly ListPool<T> Shared = new ListPool<T>();

		readonly Stack<List<T>> m_Pool;

		public ListPool () {
			m_Pool = new Stack<List<T>>(kMaxBucketSize);
		}

		/// <summary>
		/// Rent list from pool.
		/// </summary>
		/// <returns></returns>
		public List<T> Rent () {
			lock (m_Pool) {
				if (m_Pool.Count > 0) {
					return m_Pool.Pop();
				}
				return new List<T>();
			}
		}

		/// <summary>
		/// Return the list to the pool.
		/// </summary>
		/// <param name="list"></param>
		public void Return (List<T> list) {
			if (list == null) {
				return;
			}

			list.Clear();

			lock (m_Pool) {
				if (m_Pool.Count < kMaxBucketSize) {
					m_Pool.Push(list);
				}
			}
		}

		/// <summary>
		/// Return the list to the pool and set list reference to null.
		/// </summary>
		/// <param name="list"></param>
		public void Return (ref List<T> list) {
			Return(list);
			list = null;
		}

		public void Clear () {
			lock (m_Pool) {
				m_Pool.Clear();
			}
		}

	}
}