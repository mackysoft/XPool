using System;
using System.Collections.Generic;
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

		public T Original => m_Original;

		public T Rent () {
			T instance = GetPooledInstance();
			if (instance == null) {
				instance = UnityObject.Instantiate(m_Original);
			}
			return instance;
		}

		public void Return (T instance) {
			if (instance == null) {
				throw new ArgumentNullException(nameof(instance));
			}
			if (m_Pool.Count == m_Capacity) {
				return;
			}
			m_Pool.Enqueue(instance);
		}

		public void ReleaseInstances (int keep) {
			if ((keep < 0) || (keep > m_Capacity)) {
				throw new ArgumentOutOfRangeException(nameof(keep));
			}

			if (keep != 0) {
				for (int i = m_Pool.Count - keep;i > 0;i--) {
					T instance = m_Pool.Dequeue();
					if (instance != null) {
						UnityObject.Destroy(instance);
					}
				}
			}
			else {
				while (m_Pool.Count > 0) {
					T instance = m_Pool.Dequeue();
					if (instance != null) {
						UnityObject.Destroy(instance);
					}
				}
			}

		}

		protected T GetPooledInstance () {
			T instance = null;
			while ((m_Pool.Count > 0) && (instance == null)) {
				instance = m_Pool.Dequeue();
			}
			return instance;
		}
		
	}
}