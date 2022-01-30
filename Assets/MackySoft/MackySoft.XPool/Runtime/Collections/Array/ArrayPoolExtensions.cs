using System;
using System.Collections.Generic;

namespace MackySoft.XPool.Collections {
	public static class ArrayPoolExtensions {

		/// <summary>
		/// <para> Convert enumerable to array. Array are returned from <see cref="ArrayPool{T}"/>. </para>
		/// <para> The array length is not always accurate. </para>
		/// </summary>
		public static T[] ToArrayFromPool<T> (this IEnumerable<T> source) {
			return ToArrayFromPool(source,out _);
		}

		/// <summary>
		/// <para> Convert enumerable to array. Array are returned from <see cref="ArrayPool{T}"/>. </para>
		/// <para> The array length is not always accurate. </para>
		/// </summary>
		/// <param name="count"> Number of elements in source. </param>
		public static T[] ToArrayFromPool<T> (this IEnumerable<T> source,out int count) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			// Tries to cast source to the collection interfaces.
			if (source is IList<T> list) {
				count = list.Count;
				return ToArrayFromPoolInternal(list);
			}
			if (source is IReadOnlyList<T> readonlyList) {
				count = readonlyList.Count;
				return ToArrayFromPoolInternal(readonlyList);
			}
			if (source is ICollection<T> collection) {
				count = collection.Count;
				return ToArrayFromPoolInternal(collection);
			}
			if (source is IReadOnlyCollection<T> readonlyCollection) {
				count = readonlyCollection.Count;
				return ToArrayFromPoolInternal(readonlyCollection);
			}

			T[] array = ArrayPool<T>.Shared.Rent(32);
			count = 0;
			foreach (T item in source) {
				ArrayPoolUtility.EnsureCapacity(ref array,count);
				array[count] = item;
				count++;
			}
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (IReadOnlyList<T> source) {
			T[] array = ArrayPool<T>.Shared.Rent(source.Count);
			for (int i = 0;source.Count > i;i++) {
				array[i] = source[i];
			}
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (IList<T> source) {
			T[] array = ArrayPool<T>.Shared.Rent(source.Count);
			source.CopyTo(array,0);
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (IReadOnlyCollection<T> source) {
			T[] array = ArrayPool<T>.Shared.Rent(source.Count);
			int i = 0;
			foreach (T item in source) {
				array[i] = item;
				i++;
			}
			return array;
		}

		static T[] ToArrayFromPoolInternal<T> (ICollection<T> source) {
			T[] array = ArrayPool<T>.Shared.Rent(source.Count);
			source.CopyTo(array,0);
			return array;
		}

	}
}