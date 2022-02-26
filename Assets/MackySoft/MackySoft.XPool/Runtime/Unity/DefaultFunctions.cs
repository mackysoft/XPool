using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MackySoft.XPool.Unity {
	public static class DefaultFunctions {

		public static Action<T> Destroy<T> () where T : UnityObject {
			return CachedFunctions<T>.Destroy;
		}

		static class CachedFunctions<T> where T : UnityObject {

			public static readonly Action<T> Destroy = instance => {
#if UNITY_EDITOR
				if (Application.isPlaying) {
					UnityObject.Destroy(instance);
				}
				else {
					UnityObject.DestroyImmediate(instance);
				}
#else
				UnityObject.Destroy(instance);
#endif
			};
		}

	}
}