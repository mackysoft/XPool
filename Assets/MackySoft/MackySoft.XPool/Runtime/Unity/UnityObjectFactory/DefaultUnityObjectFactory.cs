using System;
using UnityEngine;
using MackySoft.XPool.Internal;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public sealed class DefaultUnityObjectFactory<T> : IUnityObjectFactory<T> where T : UnityObject {

		[Tooltip("The original object from which the pool will instantiate a new instance.")]
		[SerializeField]
		T m_Original;

		/// <summary>
		/// The original object from which the pool will instantiate a new instance.
		/// </summary>
		public T Original => m_Original;

		public DefaultUnityObjectFactory () {
		}

		public DefaultUnityObjectFactory (T original) {
			m_Original = (original != null) ? original : throw Error.ArgumentNullException(nameof(original));
		}

		public T Factory () {
			return UnityObject.Instantiate(m_Original);
		}

		public T Factory (Vector3 position,Quaternion rotation,Transform parent = null) {
			return UnityObject.Instantiate(m_Original,position,rotation,parent);
		}

		public T Factory (Transform parent,bool worldPositionStays) {
			return UnityObject.Instantiate(m_Original,parent,worldPositionStays);
		}
	}
}