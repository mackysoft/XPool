using System;

namespace MackySoft.XPool.Timers {
	public interface IReadOnlyTimer {
		event Action OnElapsed;
	}

	public interface ITimer : IReadOnlyTimer {
		bool Tick (float deltaTime);
	}
}