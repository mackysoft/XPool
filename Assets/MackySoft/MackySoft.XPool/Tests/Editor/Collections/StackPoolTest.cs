using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using MackySoft.XPool.Tests;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace MackySoft.XPool.Collections.Tests {
	public class StackPoolTest {

		StackPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new StackPool<Unit>();
		}

		[Test]
		public void Rent_does_not_allocate () {
			WarmUp();

			Assert.That(() => {
				m_Pool.Rent();
			},Is.Not.AllocatingGCMemory());
		}

		[Test]
		public void Return_does_not_allocate () {
			WarmUp();

			var array = m_Pool.Rent();
			Assert.That(() => {
				m_Pool.Return(array);
			},Is.Not.AllocatingGCMemory());
		}

		void WarmUp () {
			m_Pool.Return(m_Pool.Rent());
		}

	}
}