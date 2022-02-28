using System;
using MackySoft.XPool.Internal;

namespace MackySoft.XPool {

	/// <summary>
	/// Interface provides that basic features of pool.
	/// </summary>
	/// <typeparam name="T"> Type of instance to pool. </typeparam>
	public interface IPool<T> {

		int Capacity { get; }

		int Count { get; }

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
				throw Error.ArgumentNullException(nameof(pool));
			}
			pool.ReleaseInstances(0);
		}

		/// <summary>
		/// Return the instance to the pool and set reference to null.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static void Return<T> (this IPool<T> pool,ref T instance) {
			if (pool == null) {
				throw Error.ArgumentNullException(nameof(pool));
			}
			pool.Return(instance);
			instance = default;
		}

		/// <summary>
		/// Temporary rent an instance from pool. By using the using statement, you can safely return instance.
		/// <code>
		/// using (myPool.RentTemporary(out var instance)) {
		///		// Use instance...
		/// }
		/// </code>
		/// </summary>
		public static RentInstance<T> RentTemporary<T> (this IPool<T> pool,out T instance) {
			if (pool == null) {
				throw Error.ArgumentNullException(nameof(pool));
			}
			instance = pool.Rent();
			return new RentInstance<T>(pool,instance);
		}
	}
}