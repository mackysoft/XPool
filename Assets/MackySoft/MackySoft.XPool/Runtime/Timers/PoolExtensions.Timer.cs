using System;
using MackySoft.XPool.Timers;
using MackySoft.XPool.Internal;

namespace MackySoft.XPool {
	public static partial class PoolExtensions {

		/// <summary>
		/// Periodically release instances in the pool.
		/// </summary>
		/// <param name="pool"> Target pool. </param>
		/// <param name="interval"> Time interval to release an instances in the pool. </param>
		/// <param name="keep"> Quantity that keep pooled instances when release instances. </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static IDisposable ReleaseInstancesPeriodically (this IPool pool,float interval,int keep) {
			if (pool == null) {
				throw Error.ArgumentNullException(nameof(pool));
			}
			if (interval <= 0f) {
				throw Error.RequiredGreaterThanZero(nameof(interval));
			}
			if (keep < 0) {
				throw Error.RequiredNonNegative(nameof(keep));
			}

			var timer = new PeriodicTimer(interval);

			// Subscribe timer to the ticker.
			IDisposable subscription = TimerTicker.Instance.Subscribe(timer);

			// Bind pool to the timer.
			IDisposable binding = BindTo(pool,timer,keep);

			return Disposable.Combine(subscription,binding);
		}

		/// <summary>
		/// Bind the pool to the timer and releases an instances in the pool each time the <see cref="IReadOnlyTimer.OnElapsed"/> callback is called.
		/// </summary>
		/// <param name="pool"> Target pool. </param>
		/// <param name="timer"> Timer to be bound to the pool. </param>
		/// <param name="keep"> Quantity that keep pooled instances when release instances. </param>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static IDisposable BindTo (this IPool pool,IReadOnlyTimer timer,int keep) {
			if (pool == null) {
				throw Error.ArgumentNullException(nameof(pool));
			}
			if (timer == null) {
				throw Error.ArgumentNullException(nameof(timer));
			}
			if (keep < 0) {
				throw Error.RequiredNonNegative(nameof(keep));
			}
			return new TimerBinding(pool,timer,keep);
		}

		class TimerBinding : IDisposable {

			readonly IReadOnlyTimer m_Timer;
			readonly IPool m_Pool;
			readonly int m_Keep;
			bool m_IsDisposed;

			public TimerBinding (IPool pool,IReadOnlyTimer timer,int keep) {
				m_Pool = pool;
				m_Timer = timer;
				m_Keep = keep;

				m_Timer.OnElapsed += OnElapsed;
			}

			public void Dispose () {
				if (m_IsDisposed) {
					return;
				}
				m_IsDisposed = true;
				m_Timer.OnElapsed -= OnElapsed;
			}

			void OnElapsed () {
				m_Pool.ReleaseInstances(m_Keep > m_Pool.Capacity ? m_Pool.Capacity : m_Keep);
			}
		}
	}
}