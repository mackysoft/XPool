using System;
using System.Collections.Generic;

namespace MackySoft.XPool {

	/// <summary>
	/// Pool that create an instance from a custom factory method.
	/// </summary>
	public sealed class FactoryPool<T> : IPool<T>, IDisposable {

		readonly Func<T> m_OnCreate;
		readonly Action<T> m_OnRent;
		readonly Action<T> m_OnReturn;
		readonly Action<T> m_OnRelease;

		readonly int m_Capacity;
		readonly Queue<T> m_Pool;

#if !XPOOL_OPTIMIZE
		readonly HashSet<T> m_InPool;
#endif

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capacity"> The pool capacity. If less than or equal to 0, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		/// <param name="onCreate"> Method that create new instance. If is null, <see cref="ArgumentNullException"/> will be thrown. This method is must return not null. If returns null, <see cref="Rent"/> throw <see cref="NullReferenceException"/>. </param>
		/// <param name="onRent"> Callback that is called when <see cref="Rent"/> is successful. </param>
		/// <param name="onReturn"> Callback that is called when <see cref="Return(T)"/> is successful. </param>
		/// <param name="onRelease"> Callback that is called when capacity is exceeded and the instance cannot be returned to the pool. </param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public FactoryPool (int capacity,Func<T> onCreate,Action<T> onRent = null,Action<T> onReturn = null,Action<T> onRelease = null) {
			if (capacity <= 0) {
				throw new ArgumentOutOfRangeException(nameof(capacity));
			}

			m_OnCreate = onCreate ?? throw new ArgumentNullException(nameof(onCreate));
			m_OnRent = onRent;
			m_OnReturn = onReturn;
			m_OnRelease = onRelease;

			m_Capacity = capacity;
			m_Pool = new Queue<T>(capacity);

#if !XPOOL_OPTIMIZE
			m_InPool = new HashSet<T>();
#endif
		}

		/// <summary>
		/// Return the pooled instance. If pool is empty, create new instance and returns it.
		/// </summary>
		/// <exception cref="NullReferenceException"></exception>
		public T Rent () {
			T instance;
			if (m_Pool.Count > 0) {
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

		/// <summary>
		/// Return instance to the pool. If the capacity is exceeded, the instance will not be returned to the pool.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public void Return (T instance) {
			if (instance == null) {
				throw new ArgumentNullException(nameof(instance));
			}
			if (m_Pool.Count == m_Capacity) {
				m_OnRelease?.Invoke(instance);
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

		/// <summary>
		/// Keeps the specified quantity and releases the pooled instances.
		/// </summary>
		/// <param name="keep"> Quantity that keep pooled instances. If less than 0 or greater than capacity, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		public void ReleaseInstances (int keep) {
			if ((keep < 0) || (keep > m_Capacity)) {
				throw new ArgumentOutOfRangeException(nameof(keep));
			}

			if (keep != 0) {
				for (int i = m_InPool.Count - keep;i > 0;i--) {
					T instance = m_Pool.Dequeue();
					m_OnRelease?.Invoke(instance);
#if !XPOOL_OPTIMIZE
					m_InPool.Remove(instance);
#endif
				}
			}
			else {
				while (m_Pool.Count > 0) {
					T instance = m_Pool.Dequeue();
					m_OnRelease?.Invoke(instance);
				}
#if !XPOOL_OPTIMIZE
				m_InPool.Clear();
#endif
			}

		}

		public void Dispose () {
			ReleaseInstances(0);
		}
	}
}