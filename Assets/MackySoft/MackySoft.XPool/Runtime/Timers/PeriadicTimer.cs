using System;
using UnityEngine;

namespace MackySoft.XPool.Timers {

	[Serializable]
	public class PeriadicTimer : ITimer {

		[SerializeField]
		float m_Interval = 1f;

		float m_ElapsedTime;

		public event Action OnElapsed;

		public PeriadicTimer () {
		}

		public PeriadicTimer (float interval) {
			m_Interval = interval;
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