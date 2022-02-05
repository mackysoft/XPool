using System;

namespace MackySoft.XPool.Collections {

	// Static TemporaryList create methods
	public partial struct TemporaryList<T> {

		public static TemporaryList<T> Create () {
			return new TemporaryList<T>(ArrayPool<T>.Shared);
		}

		public static TemporaryList<T> Create (ArrayPool<T> pool) {
			if (pool == null) {
				throw new ArgumentNullException(nameof(pool));
			}
			return Create();
		}

	}
}