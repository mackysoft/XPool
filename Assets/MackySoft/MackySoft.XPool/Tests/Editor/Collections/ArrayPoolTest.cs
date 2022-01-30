using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MackySoft.XPool.Collections.Tests {
	public class ArrayPoolTest {

		public class Unit {
			public static readonly Unit Default = new Unit();
		}

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void ReturnedArrayLengthIsNextPowerOfTwo ([Random(0,1048576,100)] int size) {
			Assert.AreEqual(Mathf.NextPowerOfTwo(size),m_Pool.Rent(size).Length);
		}

		[Test]
		public void RentDoesNotAllocate () {
			WarmUp(m_Pool,5);

			Assert.That(() => {
				m_Pool.Rent(5);
			},Is.Not.AllocatingGCMemory());
		}

		static void WarmUp<T> (ArrayPool<T> pool,int size) {
			pool.Return(pool.Rent(size));
		}

	}
}