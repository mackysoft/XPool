using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {

	[Serializable]
	public class GameObjectPool : UnityObjectPool<GameObject>, IHierarchicalUnityObjectPool<GameObject> {
		public GameObject Rent (Vector3 position,Quaternion rotation,Transform parent = null) {
			GameObject instance = GetPooledInstance();
			if (instance != null) {
				Transform transform = parent.transform;
				transform.SetParent(parent);
				transform.SetPositionAndRotation(position,rotation);
			}
			else {
				instance = UnityObject.Instantiate(m_Original,position,rotation,parent);
				m_OnCreate?.Invoke(instance);
			}
			m_OnRent?.Invoke(instance);
			return instance;
		}

		public GameObject Rent (Transform parent,bool worldPositionStays) {
			GameObject instance = GetPooledInstance();
			if (instance != null) {
				instance.transform.SetParent(parent,worldPositionStays);
			}
			else {
				instance = UnityObject.Instantiate(m_Original,parent,worldPositionStays);
				m_OnCreate?.Invoke(instance);
			}
			m_OnRent?.Invoke(instance);
			return instance;
		}
	}
}