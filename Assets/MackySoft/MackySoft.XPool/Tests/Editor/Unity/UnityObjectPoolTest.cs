using System;
using NUnit.Framework;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityAssert = UnityEngine.Assertions.Assert;
using MackySoft.XPool.Tests;

namespace MackySoft.XPool.Unity.Tests {

	public class UnityObjectPoolTest {

		GameObject m_Original;

		[OneTimeSetUp]
		public void OneTimeSetUp () {
			m_Original = GameObject.CreatePrimitive(PrimitiveType.Cube);
		}

		[Test]
		public void Throw_ArgumentNullException_if_original_is_null () {
			Assert.Throws<ArgumentNullException>(() => new UnityObjectPool<GameObject>(null,8));
		}

		[Test]
		public void Throw_ArgumentOutOfRangeException_if_capacity_is_less_than_zero () {
			Assert.Throws<ArgumentOutOfRangeException>(() => new UnityObjectPool<GameObject>(m_Original,-1));
			
			// Does not throw if capacity is 0.
			Assert.DoesNotThrow(() => new UnityObjectPool<GameObject>(m_Original,0));
		}

		[Test]
		public void Throw_InvaidOperationException_if_try_to_set_callback_when_there_is_instance_in_pool () {
			var pool = new UnityObjectPool<GameObject>(m_Original,8);
			pool.Return(pool.Rent());

			Assert.Throws<InvalidOperationException>(() => pool.OnCreate = _ => { });
			Assert.Throws<InvalidOperationException>(() => pool.OnRent = _ => { });
			Assert.Throws<InvalidOperationException>(() => pool.OnReturn = _ => { });
			Assert.Throws<InvalidOperationException>(() => pool.OnRelease = _ => { });
		}

		[Test]
		public void Create_new_instance_if_cant_get_pooled_instance () {
			int count = 0;
			var pool = new UnityObjectPool<GameObject>(m_Original,8) {
				OnCreate = _ => count++
			};

			var instance = pool.Rent();
			Assert.AreEqual(1,count);

			// Return instance to the pool and destroy it.
			pool.Return(instance);
			UnityObject.DestroyImmediate(instance);
			UnityAssert.IsNull(instance);

			// Since the pooled instance has been destroyed, create a new instance.
			pool.Rent();
			Assert.AreEqual(2,count);
		}

		[Test]
		public void Release_instance_if_capacity_is_exceeded () {
			bool called = false;
			var pool = new UnityObjectPool<GameObject>(m_Original,1) {
				OnRelease = _ => called = true
			};

			var instance1 = pool.Rent();
			var instance2 = pool.Rent();

			pool.Return(instance1);
			Assert.False(called);

			pool.Return(instance2);
			Assert.True(called);
		}

		public void ReleaseInstances_throw_ArgumentOutOfRangeException_if_keep_is_less_than_zero_or_greater_than_capacity () {
			var pool = new UnityObjectPool<GameObject>(m_Original,1);
			Assert.Throws<ArgumentOutOfRangeException>(() => pool.ReleaseInstances(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => pool.ReleaseInstances(2));
		}

		[Test]
		public void ReleaseInstances_is_keep_specified_quantity_and_release_instances () {
			int released = 0;
			var pool = new UnityObjectPool<GameObject>(m_Original,3) {
				OnRelease = _ => released++
			};
			pool.WarmUp(3);
			pool.ReleaseInstances(1);

			Assert.AreEqual(2,released);
		}

		[Test]
		public void ReleaseInstances_is_release_all_instances_if_keep_is_zero () {
			int released = 0;
			var pool = new UnityObjectPool<GameObject>(m_Original,2) {
				OnRelease = _ => released++
			};
			pool.WarmUp(2);
			pool.ReleaseInstances(0);

			Assert.AreEqual(2,released);
		}

	}
}