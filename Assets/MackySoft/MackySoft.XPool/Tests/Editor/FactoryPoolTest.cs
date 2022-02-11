using System;
using NUnit.Framework;

namespace MackySoft.XPool.Tests {
	public class FactoryPoolTest {

		class IdentityFunction<T> {
			public static readonly Action<T> Action = _ => { };
			public static readonly Func<T> Func = () => default;
		}
		
		[Test]
		public void Throw_ArgumentOutOfRangeException_if_capcity_less_than_or_equal_to_zero () {
			Assert.Throws<ArgumentOutOfRangeException>(() => new FactoryPool<Unit>(0,() => Unit.Default));
			Assert.Throws<ArgumentOutOfRangeException>(() => new FactoryPool<Unit>(-1,() => Unit.Default));
		}

		[Test]
		public void Throw_ArgumentNullException_if_onCreate_is_null () {
			Assert.Throws<ArgumentNullException>(() => new FactoryPool<Unit>(1,null,IdentityFunction<Unit>.Action,IdentityFunction<Unit>.Action));
		}

		[Test]
		public void Rent_returned_new_instance_if_first_Rent () {
			int created = 0;
			var pool = new FactoryPool<Unit>(2,() => {
				created++;
				return new Unit();
			});

			Unit createdInstance = pool.Rent();
			Assert.AreEqual(1,created);

			pool.Return(createdInstance);
			pool.Rent();
			Assert.AreNotEqual(2,created);
		}

		[Test]
		public void Rent_returned_pooled_instance () {
			var pool = new FactoryPool<Unit>(1,() => new Unit());
			Unit expected = pool.Rent();
			pool.Return(expected);

			Assert.AreSame(expected,pool.Rent());
		}

		[Test]
		public void Throw_NullReferenceException_if_OnCreate_returned_null () {
			var pool = new FactoryPool<Unit>(1,() => null);
			Assert.Throws<NullReferenceException>(() => pool.Rent());
		}

		[Test]
		public void onRent_is_called_if_Rent_suceeded () {
			bool called = false;
			var pool = new FactoryPool<Unit>(1,() => new Unit(),onRent: x => called = true);
			pool.Rent();
			Assert.IsTrue(called);
		}

		[Test]
		public void Not_return_instance_to_pool_if_capacity_is_exceeded () {
			int returned = 0;
			var pool = new FactoryPool<Unit>(1,() => new Unit(),onReturn: x => returned++);

			pool.Return(new Unit());
			Assert.AreEqual(1,returned);

			pool.Return(new Unit());
			Assert.AreNotEqual(2,returned);
		}

		[Test]
		public void onRelease_is_called_if_capacity_is_exceeded () {
			bool called = false;
			var pool = new FactoryPool<Unit>(1,() => new Unit(),onRelease: x => called = true);

			pool.Return(new Unit());
			Assert.IsFalse(called);

			pool.Return(new Unit());
			Assert.IsTrue(called);
		}

		[Test]
		public void onReturn_is_called_if_Return_suceeded () {
			bool called = false;
			var pool = new FactoryPool<Unit>(1,() => new Unit(),onReturn: x => called = true);
			pool.Return(new Unit());
			Assert.IsTrue(called);
		}

		[Test]
		public void onRelease_is_called_when_clear_pool () {
			int released = 0;
			var pool = new FactoryPool<Unit>(2,() => new Unit(),onRelease: x => released++);
			pool.Return(new Unit());
			pool.Return(new Unit());
			Assert.Zero(released);

			pool.Clear();
			Assert.AreEqual(2,released);
		}

	}
}