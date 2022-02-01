using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MackySoft.XPool.Collections {
	public class QueuePool<T> {

		const int kMaxPoolSize = 32;

		public static readonly QueuePool<T> Shared = new QueuePool<T>();

		readonly Stack<Queue<T>> m_Pool;

		public QueuePool () {
			m_Pool = new Stack<Queue<T>>(kMaxPoolSize);
		}

		public Queue<T> Rent () {
			lock (m_Pool) {
				if (m_Pool.Count > 0) {
					return m_Pool.Pop();
				}
				return new Queue<T>();
			}
		}

		public void Return (Queue<T> stack) {
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

		public void Return (ref Queue<T> queue) {
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