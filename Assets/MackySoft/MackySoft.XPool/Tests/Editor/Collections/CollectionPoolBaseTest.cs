using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using MackySoft.XPool.Tests;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MackySoft.XPool.Collections.Tests {
	public class CollectionPoolBaseTest {

		ListPool<Unit> m_Pool;
		
		[SetUp]
		public void Init () {
			m_Pool = new ListPool<Unit>();
		}

		[Test]
		public void Rent_does_not_allocate () {
			m_Pool.WarmUp(1);

			Assert.That(() => {
				m_Pool.Rent();
			},Is.Not.AllocatingGCMemory());
		}

		[Test]
		public void Return_does_not_allocate () {
			m_Pool.WarmUp(1);

			var array = m_Pool.Rent();
			Assert.That(() => {
				m_Pool.Return(array);
			},Is.Not.AllocatingGCMemory());
		}

	}
}