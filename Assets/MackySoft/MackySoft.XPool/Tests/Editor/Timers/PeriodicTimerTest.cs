using NUnit.Framework;

namespace MackySoft.XPool.Timers.Tests {
	public class PeriodicTimerTest {
		
		[Test]
		public void Invoke_OnElapsed_when_interval_is_elapsed () {
			var timer = new PeriodicTimer(1f);

			bool called = false;
			timer.OnElapsed += () => called = true;

			timer.Tick(0.9f);
			Assert.IsFalse(called);

			timer.Tick(0.1f);
			Assert.IsTrue(called);
		}

	}
}