using UnityEngine;

namespace MackySoft.XPool.Unity.ObjectModel {
	public abstract class ComponentPoolBase<T> : UnityObjectPoolBase<T>, IHierarchicalUnityObjectPool<T> where T : Component {

		public T Rent (Vector3 position,Quaternion rotation,Transform parent = null) {
			T instance = GetPooledInstance();
			if (instance != null) {
				Transform transform = parent.transform;
				transform.SetParent(parent);
				transform.SetPositionAndRotation(position,rotation);
			}
			else {
				instance = UnityEngine.Object.Instantiate(m_Original,position,rotation,parent);
				OnInstantiate(instance);
			}
			OnRent(instance);
			return instance;
		}

		public T Rent (Transform parent,bool worldPositionStays) {
			T instance = GetPooledInstance();
			if (instance != null) {
				instance.transform.SetParent(parent,worldPositionStays);
			}
			else {
				instance = UnityEngine.Object.Instantiate(m_Original,parent,worldPositionStays);
				OnInstantiate(instance);
			}
			OnRent(instance);
			return instance;
		}

	}
}
