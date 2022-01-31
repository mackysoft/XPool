using System;
using MackySoft.XPool.Collections.Internal;

namespace MackySoft.XPool.Collections {
	public static class ArrayPoolUtility {

		public const int kMinArraySize = 8;

		public static void EnsureCapacity<T> (ref T[] array,int index) {
			if (array.Length <= index) {
				int newSize = array.Length * 2;
				T[] newArray = ArrayPool<T>.Shared.Rent((index < newSize) ? newSize : (index * 2));
				Array.Copy(array,0,newArray,0,array.Length);

				ArrayPool<T>.Shared.Return(array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());

				array = newArray;
			}
		}

	}
}