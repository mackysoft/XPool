using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using MackySoft.XPool.Tests;
using Is = UnityEngine.TestTools.Constraints.Is;
using MackySoft.XPool.Collections;

namespace MackySoft.XPool.Collections.Tests {
    public class TemporaryStackTest {

        ArrayPool<Unit> m_Pool;

        [SetUp]
        public void Init () {
            m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void Create_method_return_expected_stack () {
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				Assert.Zero(stack.Count);
				Assert.Zero(stack.Capacity);
			}
			using (var stack = TemporaryStack<Unit>.Create(0,m_Pool)) {
				Assert.Zero(stack.Count);
				Assert.Zero(stack.Capacity);
			}
		}

		[Test]
		public void Push_succeeded () {
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				var item = new Unit();
				stack.Push(item);
				Assert.AreSame(item,stack.Peek());
			}
			using (var stack = TemporaryStack<Unit>.Create(8,m_Pool)) {
				while (stack.Capacity > stack.Count) {
					stack.Push(new Unit());
				}
				Assert.AreEqual(8,stack.Count);

				var item = new Unit();
				stack.Push(item);
				Assert.AreSame(item,stack.Peek());
				Assert.AreEqual(16,stack.Capacity);
			}
		}

		[Test]
		public void Pop_and_Peek_throw_InvalidOperartionException_if_stack_is_empty () {
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				Assert.IsEmpty(stack);
				Assert.Throws<InvalidOperationException>(() => stack.Pop());
				Assert.Throws<InvalidOperationException>(() => stack.Peek());
			}
		}

		[Test]
		public void Pop_succeeded () {
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				var item = new Unit();
				stack.Push(item);
				Assert.AreSame(item,stack.Pop());
			}
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				for (int i = 0;8 > i;i++) {
					stack.Push(new Unit());
				}
				for (int i = 0;8 > i;i++) {
					Unit item = stack.Peek();
					Assert.AreSame(item,stack.Pop());
				}
			}
		}

		[Test]
		public void Clear_succeeded () {
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				var item = new Unit();
				stack.Push(item);

				stack.Clear();
				Assert.IsEmpty(stack);
			}
		}

		[Test]
		public void Same_as_copied_array () {
			using (var stack = TemporaryStack<Unit>.Create(m_Pool)) {
				for (int i = 0;8 > i;i++) {
					var item = new Unit();
					stack.Push(item);
				}
				Unit[] array = new Unit[8];
				stack.CopyTo(array,0);
				CollectionAssert.AreEqual(stack,array);
			}
		}

		[Test]
		public void Array_is_null_when_disposed () {
			var stack = TemporaryStack<Unit>.Create(m_Pool);
			Assert.IsNotNull(stack.Array);

			stack.Dispose();
			Assert.IsNull(stack.Array);
		}

	}
}