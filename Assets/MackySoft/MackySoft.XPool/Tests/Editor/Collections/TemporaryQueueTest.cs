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
		public void Enqueue_succeeded () {
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				var item = new Unit();
				queue.Enqueue(item);
				Assert.AreSame(item,queue.GetElement(0));
			}
			using (var queue = TemporaryQueue<Unit>.Create(8,m_Pool)) {
				while (queue.Capacity > queue.Count) {
					queue.Enqueue(new Unit());
				}
				Assert.AreEqual(8,queue.Count);

				var item = new Unit();
				queue.Enqueue(item);
				Assert.AreSame(item,queue.GetElement(8));
				Assert.AreEqual(16,queue.Capacity);
			}
		}

		[Test]
		public void Dequeue_succeeded () {
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				var item = new Unit();
				queue.Enqueue(item);
				Assert.AreSame(item,queue.Dequeue());
			}
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				for (int i = 0;8 > i;i++) {
					var item = new Unit();
					queue.Enqueue(item);
					Assert.AreSame(item,queue.Dequeue());
				}
			}
		}

		[Test]
		public void Clear_succeeded () {
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				var item = new Unit();
				queue.Enqueue(item);

				queue.Clear();
				Assert.IsEmpty(queue);
			}
		}

		[Test]
		public void Same_as_copied_array () {
			using (var queue = TemporaryQueue<Unit>.Create(m_Pool)) {
				for (int i = 0;8 > i;i++) {
					var item = new Unit();
					queue.Enqueue(item);
				}
				Unit[] array = new Unit[8];
				queue.CopyTo(array,0);
				CollectionAssert.AreEqual(queue,array);
			}
		}

		[Test]
		public void Array_is_null_when_disposed () {
			var queue = TemporaryQueue<Unit>.Create(m_Pool);
			Assert.IsNotNull(queue.Array);

			queue.Dispose();
			Assert.IsNull(queue.Array);
		}

	}
}