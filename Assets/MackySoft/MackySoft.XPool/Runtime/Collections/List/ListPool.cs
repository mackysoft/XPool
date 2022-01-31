using System;
using System.Collections.Generic;

namespace MackySoft.XPool.Collections {
	public class ListPool<T> {

		public static readonly ListPool<T> Shared = new ListPool<T>();

		readonly Stack<List<T>> m_Pool;

		public ListPool () {
			m_Pool = new Stack<List<T>>();
		}

		/// <summary>
		/// キャパシティが設定されていないListを返す
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

		public void Return (List<T> list) {
			if (list == null) {
				return;
			}

			lock (m_Pool) {
				m_Pool.Push(list);
			}
		}

		public void Clear () {
			lock (m_Pool) {
				m_Pool.Clear();
			}
		}

	}
}