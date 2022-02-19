using System;

namespace MackySoft.XPool {
	public struct RentInstance<T> : IDisposable {

		readonly T m_Instance;
		readonly IPool<T> m_Pool;

		internal RentInstance (IPool<T> pool,T instance) {
			m_Pool = pool;
			m_Instance = instance;
		}

		public void Dispose () => m_Pool.Return(m_Instance);
	}
}