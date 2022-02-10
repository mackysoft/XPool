using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using MackySoft.XPool.Tests;

namespace MackySoft.XPool.Collections.Tests {
	public class TemporaryArrayTest {

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void Create_method_return_expected_array () {
			var array = TemporaryArray<Unit>.Create(6,m_Pool);
			Assert.AreEqual(6,array.Length);
			Assert.AreEqual(8,array.Capacity);
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

		[Test]
		public void Same_as_copied_temporary_array () {
			var collection = new int[] { 1,2,3 };
			var sourceArray = TemporaryArray<int>.From(collection);
			var copiedArray = TemporaryArray<int>.From(sourceArray);
			CollectionAssert.AreEqual(sourceArray,copiedArray);
		}

	}
}