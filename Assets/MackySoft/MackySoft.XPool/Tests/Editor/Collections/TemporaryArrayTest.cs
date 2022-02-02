using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MackySoft.XPool.Collections.Tests {
	public class TemporaryArrayTest {

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void Array_is_null_when_disposed () {
			var array = TemporaryArray<Unit>.Create(0,m_Pool);
			array.Dispose();
			Assert.IsNull(array.Array);
		}

		[Test]
		public void Same_as_copied_collection () {
			var collection = new int[] { 1,2,3 };
			var array = TemporaryArray<int>.From(collection,ArrayPool<int>.Shared);
			CollectionAssert.AreEqual(collection,array);
		}

	}
}