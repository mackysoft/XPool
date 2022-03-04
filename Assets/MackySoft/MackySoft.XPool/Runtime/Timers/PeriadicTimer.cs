using System;
using UnityEngine;

namespace MackySoft.XPool.Timers {

	/// <summary>
	/// Timer that resets each time a interval of time elapses.
	/// </summary>
	[Serializable]
	public class PeriodicTimer : ITimer {

		[SerializeField]
		float m_Interval = 1f;

		float m_ElapsedTime;

		public event Action OnElapsed;

		public float Interval {
			get => m_Interval;
			set => m_Interval = (value > 0f) ? value : 0.01f;
		}

		public PeriodicTimer () {
		}

		public PeriodicTimer (float interval) {
			Interval = interval;
		}

		public bool Tick (float deltaTime) {
			m_ElapsedTime += deltaTime;
			if (m_ElapsedTime >= m_Interval) {
				m_ElapsedTime -= m_Interval;
				OnElapsed?.Invoke();
				return true;
			}
			return false;
		}
	}
}