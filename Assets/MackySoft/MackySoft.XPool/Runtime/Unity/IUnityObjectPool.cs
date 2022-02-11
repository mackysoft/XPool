using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IUnityObjectPool<T> : IPool<T> where T : UnityObject {
		T Original { get; }
	}

	/// <summary>
	/// Interface that provides rent method for GameObject or components.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IHierarchicalUnityObjectPool<T> : IUnityObjectPool<T> where T : UnityObject {
		T Rent (Vector3 position,Quaternion rotation,Transform parent = null);
		T Rent (Transform parent,bool worldPositionStays);
	}
}