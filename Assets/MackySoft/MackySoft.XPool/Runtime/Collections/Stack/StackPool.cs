using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MackySoft.XPool.Collections {
	public class StackPool<T> {

		const int kMaxPoolSize = 32;

		public static readonly StackPool<T> Shared = new StackPool<T>();

		readonly Stack<Stack<T>> m_Pool;

		public StackPool () {
			m_Pool = new Stack<Stack<T>>(kMaxPoolSize);
		}

		public Stack<T> Rent () {
			lock (m_Pool) {
				if (m_Pool.Count > 0) {
					return m_Pool.Pop();
				}
				return new Stack<T>();
			}
		}

		public void Return (Stack<T> stack) {
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

		public void Return (ref Stack<T> stack) {
			Return(stack);
			stack = null;
		}

		public void Clear () {
			lock (m_Pool) {
				m_Pool.Clear();
			}
		}

	}
}