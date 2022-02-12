using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity.ObjectModel {
	public abstract class UnityObjectPoolBase<T> : IUnityObjectPool<T> where T : UnityObject {
		
		[SerializeField]
		protected T m_Original;

		[SerializeField]
		int m_Capacity;

		Queue<T> m_Pool;

		public T Original => m_Original;

		public T Rent () {
			T instance = GetPooledInstance();
			if (instance == null) {
				instance = UnityObject.Instantiate(m_Original);
				OnCreate(instance);
			}
			OnRent(instance);
			return instance;
		}

		public void Return (T instance) {
			if (instance == null) {
				throw new ArgumentNullException(nameof(instance));
			}
			if (m_Pool.Count == m_Capacity) {
				OnRelease(instance);
				return;
			}
			m_Pool.Enqueue(instance);
			OnReturn(instance);
		}

		public void ReleaseInstances (int keep) {
			if ((keep < 0) || (keep > m_Capacity)) {
				throw new ArgumentOutOfRangeException(nameof(keep));
			}

			if (keep != 0) {
				for (int i = m_Pool.Count - keep;i > 0;i--) {
					T instance = m_Pool.Dequeue();
					if (instance != null) {
						OnRelease(instance);
					}
				}
			}
			else {
				while (m_Pool.Count > 0) {
					T instance = m_Pool.Dequeue();
					if (instance != null) {
						OnRelease(instance);
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

		protected abstract void OnCreate (T instance);
		protected abstract void OnRent (T instance);
		protected abstract void OnReturn (T instance);
		protected abstract void OnRelease (T instance);
	}
}
