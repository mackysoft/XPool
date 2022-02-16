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

		[Test]
		public void Create_method_return_expected_queue () {
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				Assert.Zero(queue.Count);
				Assert.Zero(queue.Capacity);
			}
			using (var queue = TemporaryQueue<Unit>.Create(0,m_Pool)) {
				Assert.Zero(queue.Count);
				Assert.Zero(queue.Capacity);
			}
		}

		[Test]
		public void Enqueue () {
			// ただしくEnqueue出来ているかどうか（最後尾に挿入できているか）
			// Lastが正しく更新されているかどうか
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				var item = new Unit();
				queue.Enqueue(item);
				Debug.Log(queue.Capacity);
				Assert.AreSame(item,queue.GetElement(0));
			}
		}

	}
}