using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {

	[Serializable]
	public class ComponentPool<T> : UnityObjectPool<T>, IHierarchicalUnityObjectPool<T> where T : Component {

		public T Rent (Vector3 position,Quaternion rotation,Transform parent = null) {
			T instance = GetPooledInstance();
			if (instance != null) {
				Transform transform = parent.transform;
				transform.SetParent(parent);
				transform.SetPositionAndRotation(position,rotation);
			}
			else {
				instance = UnityObject.Instantiate(m_Original,position,rotation,parent);
			}
			return instance;
		}

		public T Rent (Transform parent,bool worldPositionStays) {
			T instance = GetPooledInstance();
			if (instance != null) {
				instance.transform.SetParent(parent,worldPositionStays);
			}
			else {
				instance = UnityObject.Instantiate(m_Original,parent,worldPositionStays);
			}
			return instance;
		}

	}
}