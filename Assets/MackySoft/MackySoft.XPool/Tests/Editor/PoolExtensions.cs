using System;

namespace MackySoft.XPool.Tests {
	public static class PoolExtensions {
		public static void WarmUp<T> (this IPool<T> pool,int count) {
			if (pool == null) {
				throw new ArgumentNullException(nameof(pool));
			}
			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			T[] array = new T[count];
			for (int i = 0;i < count;i++) {
				array[i] = pool.Rent();
			}
			for (int i = 0;i < count;i++) {
				pool.Return(array[i]);
			}
		}
	}
}