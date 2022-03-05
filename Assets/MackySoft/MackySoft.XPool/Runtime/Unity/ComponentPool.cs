using System;
using UnityEngine;

namespace MackySoft.XPool.Unity {

	/// <summary>
	/// A pool for Component.
	/// </summary>
	[Serializable]
	public class ComponentPool<T> : UnityObjectPool<T>, IHierarchicalUnityObjectPool<T> where T : Component {

		public ComponentPool () : base() {
		}

		/// <param name="original"> The original object from which the pool will instantiate a new instance. </param>
		/// <param name="capacity"> The pool capacity. If less than 0, <see cref="ArgumentOutOfRangeException"/> will be thrown. </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public ComponentPool (T original,int capacity) : base(original,capacity) {
		}

		public T Rent (Vector3 position,Quaternion rotation,Transform parent = null) {
			T instance = GetPooledInstance();
			if (instance != null) {
				Transform transform = parent.transform;
				transform.SetParent(parent);
				transform.SetPositionAndRotation(position,rotation);
			}
			else {
				instance = m_Factory.Factory(position,rotation,parent);
				m_OnCreate?.Invoke(instance);;
			}
			m_OnRent?.Invoke(instance);
			return instance;
		}

		public T Rent (Transform parent,bool worldPositionStays) {
			T instance = GetPooledInstance();
			if (instance != null) {
				instance.transform.SetParent(parent,worldPositionStays);
			}
			else {
				instance = m_Factory.Factory(parent,worldPositionStays);
				m_OnCreate?.Invoke(instance);
			}
			m_OnRent?.Invoke(instance);
			return instance;
		}

	}
}