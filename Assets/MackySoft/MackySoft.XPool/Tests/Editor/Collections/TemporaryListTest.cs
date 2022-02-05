using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MackySoft.XPool.Collections.Tests {
	public class TemporaryListTest {

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void Create_method_return_expected_list () {
			var array = TemporaryList<Unit>.Create(6,m_Pool);
			Assert.AreEqual(0,array.Count);
			Assert.AreEqual(8,array.Capacity);
		}

		[Test]
		public void Array_is_null_when_disposed () {
			var array = TemporaryList<Unit>.Create(m_Pool);
			array.Dispose();
			Assert.IsNull(array.Array);
		}

		[Test]
		public void Same_as_copied_collection () {
			var collection = new int[] { 1,2,3 };
			var array = TemporaryList<int>.From(collection);
			CollectionAssert.AreEqual(collection,array);
		}

		[Test]
		public void Add_does_not_allocate () {
			var list = TemporaryList<Unit>.Create(8,m_Pool);

			// Warm up
			list.Add(Unit.Default);

			Assert.That(() => {
				list.Add(Unit.Default);
			},Is.Not.AllocatingGCMemory());
		}

	}
}