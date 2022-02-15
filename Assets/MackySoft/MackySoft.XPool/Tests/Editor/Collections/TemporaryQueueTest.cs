using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using MackySoft.XPool.Tests;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MackySoft.XPool.Collections.Tests {
	public class TemporaryQueueTest {

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

	}
}