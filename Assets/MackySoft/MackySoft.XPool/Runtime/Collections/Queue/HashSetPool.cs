using System.Collections.Generic;

namespace MackySoft.XPool.Collections {
	public class HashSetPool<T> {
		const int kMaxPoolSize = 32;

		public static readonly HashSetPool<T> Shared = new HashSetPool<T>();

		readonly Stack<HashSet<T>> m_Pool;

		public HashSetPool () {
			m_Pool = new Stack<HashSet<T>>(kMaxPoolSize);
		}

		public HashSet<T> Rent () {
			lock (m_Pool) {
				if (m_Pool.Count > 0) {
					return m_Pool.Pop();
				}
				return new HashSet<T>();
			}
		}

		public void Return (HashSet<T> stack) {
			if (stack == null) {
				return;
			}

			stack.Clear();

			lock (m_Pool) {
				if (m_Pool.Count < kMaxPoolSize) {
					m_Pool.Push(stack);
				}
			}
		}

		public void Return (ref HashSet<T> queue) {
			Return(queue);
			queue = null;
		}

		public void Clear () {
			lock (m_Pool) {
				m_Pool.Clear();
			}
		}
	}
}