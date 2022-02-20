using System;
using System.Collections.Generic;
using MackySoft.XPool.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {

	[Serializable]
	public class UnityObjectPool<T> : IUnityObjectPool<T> where T : UnityObject {

		[SerializeField]
		protected T m_Original;

		[SerializeField]
		int m_Capacity;

		Queue<T> m_Pool;

		protected Action<T> m_OnCreate;
		protected Action<T> m_OnRent;
		protected Action<T> m_OnReturn;
		protected Action<T> m_OnRelease;

		public T Original => m_Original;

		public T Rent () {
			T instance = GetPooledInstance();
			if (instance == null) {
				instance = UnityObject.Instantiate(m_Original);
				m_OnCreate?.Invoke(instance);
			}
			m_OnRent?.Invoke(instance);
			return instance;
		}

		public void Return (T instance) {
			if (instance == null) {
				throw Error.ArgumentNullException(nameof(instance));
			}
			if (m_Pool.Count == m_Capacity) {
				m_OnRelease.Invoke(instance);
				return;
			}
			m_Pool.Enqueue(instance);
			m_OnReturn?.Invoke(instance);
		}

		public void ReleaseInstances (int keep) {
			if ((keep < 0) || (keep > m_Capacity)) {
				throw new ArgumentOutOfRangeException(nameof(keep));
			}

			if (keep != 0) {
				for (int i = m_Pool.Count - keep;i > 0;i--) {
					T instance = m_Pool.Dequeue();
					if (instance != null) {
						m_OnRelease.Invoke(instance);
					}
				}
			}
			else {
				while (m_Pool.Count > 0) {
					T instance = m_Pool.Dequeue();
					if (instance != null) {
						m_OnRelease.Invoke(instance);
					}
				}
			}
		}

		/// <summary>
		/// Try to get an instance until the pool is empty or an instance can be retrieved.
		/// This is because <see cref="UnityObject"/> can become null externally due to the <see cref="UnityObject.Destroy(UnityObject)"/> method.
		/// </summary>
		protected T GetPooledInstance () {
			T instance = null;
			while ((m_Pool.Count > 0) && (instance == null)) {
				instance = m_Pool.Dequeue();
			}
			return instance;
		}
		
	}
}