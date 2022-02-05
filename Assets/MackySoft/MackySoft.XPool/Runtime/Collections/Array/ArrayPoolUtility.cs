using System;
using MackySoft.XPool.Collections.Internal;

namespace MackySoft.XPool.Collections {
	public static class ArrayPoolUtility {

		public const int kMinArraySize = 8;

		public static void EnsureCapacity<T> (ref T[] array,int index,ArrayPool<T> pool) {
			if (array.Length <= index) {
				int newSize = (array.Length != 0) ? array.Length * 2 : kMinArraySize;
				T[] newArray = pool.Rent((index < newSize) ? newSize : (index * 2));
				Array.Copy(array,0,newArray,0,array.Length);

				pool.Return(array,!RuntimeHelpers.IsWellKnownNoReferenceContainsType<T>());

				array = newArray;
			}
		}

	}
}