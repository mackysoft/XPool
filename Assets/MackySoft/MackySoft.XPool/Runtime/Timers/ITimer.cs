using System;

namespace MackySoft.XPool.Timers {

	public interface IReadOnlyTimer {
		/// <summary>
		/// Called when the timer has elapsed.
		/// </summary>
		event Action OnElapsed;
	}

	public interface ITimer : IReadOnlyTimer {
		/// <summary>
		/// Update timer. Return true if timer has elapsed.
		/// </summary>
		bool Tick (float deltaTime);
	}
}