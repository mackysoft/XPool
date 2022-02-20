using System;
using System.Collections.Generic;
using MackySoft.XPool.Internal;

namespace MackySoft.XPool.ObjectModel {

	/// <summary>
	/// Provides basic features of pool.
	/// </summary>
	public abstract class PoolBase<T> : IPool<T> {

		readonly int m_Capacity;
		readonly Queue<T> m_Pool;

#if !XPOOL_OPTIMIZE
		readonly HashSet<T> m_InPool;
#endif

		/// <summary>
		/// Initialize the pool with capacity. The inherited class must call this constructor.
		/// </summary>
		/// <param name="capacity"> The pool capacity. If less than or equal to 0, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		protected PoolBase (int capacity) {
			if (capacity < 0) {
				throw Error.RequiredNonNegative(nameof(capacity));
			}
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
				instance = Factory() ?? throw new NullReferenceException($"{nameof(Factory)} method must not return null.");
			}

			OnRent(instance);
			return instance;
		}

		/// <summary>
		/// Return instance to the pool. If the capacity is exceeded, the instance will not be returned to the pool.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public void Return (T instance) {
			if (instance == null) {
				throw Error.ArgumentNullException(nameof(instance));
			}
			if (m_Pool.Count == m_Capacity) {
				OnRelease(instance);
				return;
			}
#if !XPOOL_OPTIMIZE
			if (!m_InPool.Add(instance)) {
				return;
			}
#endif
			m_Pool.Enqueue(instance);
			OnReturn(instance);
		}

		/// <summary>
		/// Keeps the specified quantity and releases the pooled instances.
		/// </summary>
		/// <param name="keep"> Quantity that keep pooled instances. If less than 0 or greater than capacity, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		public void ReleaseInstances (int keep) {
			if ((keep < 0) || (keep > m_Capacity)) {
				throw Error.ArgumentOutOfRangeOfCollection(nameof(keep));
			}

			if (keep != 0) {
				for (int i = m_Pool.Count - keep;i > 0;i--) {
					T instance = m_Pool.Dequeue();
					OnRelease(instance);
#if !XPOOL_OPTIMIZE
					m_InPool.Remove(instance);
#endif
				}
			}
			else {
				while (m_Pool.Count > 0) {
					T instance = m_Pool.Dequeue();
					OnRelease(instance);
				}
#if !XPOOL_OPTIMIZE
				m_InPool.Clear();
#endif
			}
		}

		protected abstract T Factory ();
		protected abstract void OnRent (T instance);
		protected abstract void OnReturn (T instance);
		protected abstract void OnRelease (T instance);

	}
}