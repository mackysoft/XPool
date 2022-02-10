using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using MackySoft.XPool.Tests;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MackySoft.XPool.Collections.Tests {
	public class ArrayPoolTest {

		static readonly int[] kArraySizes = new int[] {
			8,16,32,64,128,256,512,1024,2048,4096,8192,16384,32768,65536,131072,262144,524288,1048576
		};

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void Throw_if_required_size_is_negative () {
			Assert.Throws(typeof(ArgumentOutOfRangeException),() => m_Pool.Rent(-1));
		}

		[Test]
		public void Return_empty_array_if_required_size_is_zero () {
			Assert.IsEmpty(m_Pool.Rent(0));
		}

		[Test]
		public void Return_minimum_size_array_if_required_size_less_than_minimum_array_size () {
			Assert.AreEqual(ArrayPoolUtility.kMinArraySize,m_Pool.Rent(1).Length);
		}

		[Test, TestCaseSource(nameof(kArraySizes))]
		public void Returned_array_size_is_next_power_of_two (int powerOfTwoSize) {
			// 8 -> 8
			Assert.AreEqual(powerOfTwoSize,m_Pool.Rent(powerOfTwoSize).Length);

			// 7 -> 8
			Assert.AreEqual(powerOfTwoSize,m_Pool.Rent(powerOfTwoSize - 1).Length);

			// 9 -> 16
			Assert.AreEqual(powerOfTwoSize * 2,m_Pool.Rent(powerOfTwoSize + 1).Length);
		}

		[Test]
		public void Rent_does_not_allocate () {
			WarmUp(5);

			Assert.That(() => {
				m_Pool.Rent(5);
			},Is.Not.AllocatingGCMemory());
		}

		[Test]
		public void Return_does_not_allocate () {
			WarmUp(5);

			var array = m_Pool.Rent(5);
			Assert.That(() => {
				m_Pool.Return(array);
			},Is.Not.AllocatingGCMemory());
		}

		void WarmUp (int size) {
			m_Pool.Return(m_Pool.Rent(size));
		}

	}
}