using System;
using System.Collections.Generic;
using MackySoft.XPool.Internal;
using UnityEngine;

namespace MackySoft.XPool.Timers {

	/// <summary>
	/// A component that keeps updating the registered timers.
	/// </summary>
	public class TimerTicker : MonoBehaviour {

		public static TimerTicker Instance {
			get {
				if (s_Instance == null) {
					var go = new GameObject("[XPool] TimerTicker");
					s_Instance = go.AddComponent<TimerTicker>();
					DontDestroyOnLoad(go);
				}
				return s_Instance;
			}
		}

		static TimerTicker s_Instance;

		readonly List<ITimer> m_Timers = new List<ITimer>();

		void Awake () {
			if (s_Instance == null) {
				s_Instance = this;
			}
		}

		void OnDestroy () {
			if (s_Instance == this) {
				s_Instance = null;
			}
		}

		void Update () {
			float deltaTime = Time.deltaTime;
			for (int i = 0;i < m_Timers.Count;i++) {
				m_Timers[i].Tick(deltaTime);
			}
		}

		public void Register (ITimer timer) {
			if ((timer == null) || m_Timers.Contains(timer)) {
				return;
			}
			m_Timers.Add(timer);
		}

		public void Unregister (ITimer timer) {
			m_Timers.Remove(timer);
		}

		public IDisposable Subscribe (ITimer timer) {
			return (timer != null) ? new TimerRegistration(timer,this) : throw Error.ArgumentNullException(nameof(timer));
		}

		class TimerRegistration : IDisposable {

			readonly ITimer m_Timer;
			readonly TimerTicker m_TimerTicker;
			bool m_IsDisposed;

			public TimerRegistration (ITimer timer,TimerTicker timerTicker) {
				m_Timer = timer;
				m_TimerTicker = timerTicker;

				m_TimerTicker.Register(timer);
			}

			public void Dispose () {
				if (m_IsDisposed) {
					return;
				}
				m_IsDisposed = true;

				if (m_TimerTicker != null) {
					m_TimerTicker.Unregister(m_Timer);
				}
			}
		}
	}
}