using System;
using MackySoft.XPool.Internal;
using MackySoft.XPool.Timers;

namespace MackySoft.XPool {
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

		public static IDisposable ReleaseInstancesPeriodically (this IPool pool,float interval,int keep) {
			var timer = new PeriadicTimer(interval);
			IDisposable subscription = TimerTicker.Instance.Subscribe(timer);
			IDisposable binding = BindTo(pool,timer,keep);
			return Disposable.Combine(subscription,binding);
		}

		public static IDisposable BindTo (this IPool pool,IReadOnlyTimer timer,int keep) {
			return new TimerBinding(pool,timer,keep);
		}

		class TimerBinding : IDisposable {

			readonly IReadOnlyTimer m_Timer;
			readonly IPool m_Pool;
			int m_Keep;

			public int Keep { get => m_Keep; set => m_Keep = value; }

			public TimerBinding (IPool pool,IReadOnlyTimer timer,int keep) {
				if (keep < 0) {
					throw Error.ArgumentOutOfRangeOfCollection(nameof(keep));
				}
				m_Pool = pool ?? throw Error.ArgumentNullException(nameof(pool));
				m_Timer = timer ?? throw Error.ArgumentNullException(nameof(timer));
				m_Keep = keep;

				m_Timer.OnElapsed += OnElapsed;
			}

			public void Dispose () {
				m_Timer.OnElapsed -= OnElapsed;
			}

			void OnElapsed () {
				m_Pool.ReleaseInstances(m_Keep > m_Pool.Capacity ? m_Pool.Capacity : m_Keep);
			}
		}

		
	}
}