using System;
using System.Collections.Generic;

namespace MackySoft.XPool.Collections {

	// Static TemporaryArray create methods
	public partial struct TemporaryArray<T> {

		/// <summary>
		/// Create a temporary array of the specified length.
		/// </summary>
		public static TemporaryArray<T> Create (int length) {
			return new TemporaryArray<T>(ArrayPool<T>.Shared.Rent(length),length);
		}

		/// <summary>
		/// <para> Create a temporary array with a length of 0. </para>
		/// <para> The length can be increased by using the <see cref="Add(T)"/>. </para>
		/// </summary>
		/// <param name="prepare"> Length of the internal array to be prepared. </param>
		public static TemporaryArray<T> CreateAsList (int prepare) {
			return new TemporaryArray<T>(ArrayPool<T>.Shared.Rent(prepare),0);
		}

		/// <summary>
		/// Create a temporary array from the elements of <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static TemporaryArray<T> From (IEnumerable<T> source) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}

			T[] array = source.ToArrayFromPool(out int count);
			return new TemporaryArray<T>(array,count);
		}

		public static TemporaryArray<T> From (TemporaryArray<T> source) {
			var result = Create(source.Length);
			for (int i = 0;source.Length > i;i++) {
				result[i] = source[i];
			}
			return result;
		}

	}

	public static class TemporaryArrayExtensions {

		/// <summary>
		/// Create a temporary array from the elements of <see cref="IEnumerable{T}"/>.
		/// </summary>
		public static TemporaryArray<T> ToTemporaryArray<T> (this IEnumerable<T> source) {
			return TemporaryArray<T>.From(source);
		}

	}
}