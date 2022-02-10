using System;
using System.Collections.Generic;

namespace MackySoft.XPool {

	/// <summary>
	/// Pool that create an instance from a custom factory method.
	/// </summary>
	public sealed class FactoryPool<T> {

		readonly Func<T> m_OnCreate;
		readonly Action<T> m_OnRent;
		readonly Action<T> m_OnReturn;

		readonly int m_Capacity;
		readonly Queue<T> m_Pool;

#if !XPOOL_OPTIMIZE
		readonly HashSet<T> m_InPool;
#endif

		public FactoryPool (int capacity,Func<T> onCreate,Action<T> onRent,Action<T> onReturn) {
			if (capacity < 0) {
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			m_OnCreate = onCreate ?? throw new ArgumentNullException(nameof(onCreate));
			m_OnRent = onRent;
			m_OnReturn = onReturn;

			m_Capacity = capacity;
			m_Pool = new Queue<T>(capacity);

#if !XPOOL_OPTIMIZE
			m_InPool = new HashSet<T>();
#endif
		}

		public T Rent () {
			T instance;
			if (m_InPool.Count > 0) {
				instance = m_Pool.Dequeue();
#if !XPOOL_OPTIMIZE
				m_InPool.Remove(instance);
#endif
			}
			else {
				instance = m_OnCreate() ?? throw new NullReferenceException($"{m_OnCreate} must not return null.");
			}
			
			m_OnRent?.Invoke(instance);
			return instance;
		}

		public void Return (T instance) {
			if (instance == null) {
				throw new ArgumentNullException(nameof(instance));
			}
			if (m_Pool.Count == m_Capacity) {
				return;
			}
#if !XPOOL_OPTIMIZE
			if (!m_InPool.Add(instance)) {
				return;
			}
#endif
			m_Pool.Enqueue(instance);
			m_OnReturn?.Invoke(instance);
		}
	}
}