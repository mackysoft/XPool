using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {
	public interface IUnityObjectFactory<out T> where T : UnityObject {
		T Factory ();
		T Factory (Vector3 position,Quaternion rotation,Transform parent = null);
		T Factory (Transform parent,bool worldPositionStays);
	}
}