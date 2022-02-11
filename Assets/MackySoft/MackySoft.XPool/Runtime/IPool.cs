using System;

namespace MackySoft.XPool {

	/// <summary>
	/// Interface provides that basic features of pool.
	/// </summary>
	/// <typeparam name="T"> Type of instance to pool. </typeparam>
	public interface IPool<T> {

		/// <summary>
		/// Return the pooled instance. If pool is empty, create new instance and returns it.
		/// </summary>
		T Rent ();

		/// <summary>
		/// Return instance to the pool.
		/// </summary>
		void Return (T instance);

		/// <summary>
		/// Keeps the specified quantity and releases the pooled instances.
		/// </summary>
		/// <param name="keep"> Quantity that keep pooled instances. </param>
		void ReleaseInstances (int keep);
	}

	public static class PoolExtensions {

		/// <summary>
		/// Release the all pooled instances.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static void Clear<T> (this IPool<T> pool) {
			if (pool == null) {
				throw new ArgumentNullException(nameof(pool));
			}
			pool.ReleaseInstances(0);
		}

		/// <summary>
		/// Return the instance to the pool and set reference to null.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static void Return<T> (this IPool<T> pool,ref T instance) {
			if (pool == null) {
				throw new ArgumentNullException(nameof(pool));
			}
			pool.Return(instance);
			instance = default;
		}

	}
}