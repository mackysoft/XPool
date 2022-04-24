using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MackySoft.XPool.Tests;

namespace MackySoft.XPool.Collections.Tests {
	public class ToArrayFromPoolTest {

		ArrayPool<Unit> m_Pool;

		[SetUp]
		public void Init () {
			m_Pool = new ArrayPool<Unit>();
		}

		[Test]
		public void Throw_if_argument_is_null () {
			Unit[] source = Array.Empty<Unit>();
			
			Assert.DoesNotThrow(() => ArrayPoolExtensions.ToArrayFromPool(source,m_Pool,out _));
			Assert.Throws<ArgumentNullException>(() => ArrayPoolExtensions.ToArrayFromPool(null,m_Pool,out _));
			Assert.Throws<ArgumentNullException>(() => ArrayPoolExtensions.ToArrayFromPool(source,null,out _));
		}

		[Test]
		public void Return_expected_array_from_IList () {
			List<Unit> source = new List<Unit>() {
				new Unit()
			};

			Unit[] result = ArrayPoolExtensions.ToArrayFromPool(source,m_Pool,out int count);

			Assert.AreEqual(source.Count,count);
			CollectionAssert.AreEqual(source,result.Take(count));
		}

		[Test]
		public void Return_expected_array_from_IReadOnlyList () {
			List<Unit> source = new List<Unit>() {
				new Unit()
			};

			Unit[] result = ArrayPoolExtensions.ToArrayFromPool(new ReadOnlyList<Unit>(source),m_Pool,out int count);

			Assert.AreEqual(source.Count,count);
			CollectionAssert.AreEqual(source,result.Take(count));
		}

		[Test]
		public void Return_expected_array_from_ICollection () {
			List<Unit> source = new List<Unit>() {
				new Unit()
			};

			Unit[] result = ArrayPoolExtensions.ToArrayFromPool(new Collection<Unit>(source),m_Pool,out int count);

			Assert.AreEqual(source.Count,count);
			CollectionAssert.AreEqual(source,result.Take(count));
		}

		[Test]
		public void Return_expected_array_from_IReadOnlyCollection () {
			List<Unit> source = new List<Unit>() {
				new Unit()
			};

			Unit[] result = ArrayPoolExtensions.ToArrayFromPool(new ReadOnlyCollection<Unit>(source),m_Pool,out int count);

			Assert.AreEqual(source.Count,count);
			CollectionAssert.AreEqual(source,result.Take(count));
		}

		class ReadOnlyList<T> : IReadOnlyList<T> {

			readonly List<T> m_Source;

			public ReadOnlyList (List<T> inner) {
				m_Source = inner;
			}

			public T this[int index] => m_Source[index];

			public int Count => m_Source.Count;

			public IEnumerator<T> GetEnumerator () => m_Source.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator () => m_Source.GetEnumerator();

		}

		class Collection<T> : ICollection<T> {

			readonly List<T> m_Source;

			public Collection (List<T> source) {
				m_Source = source;
			}

			public int Count => m_Source.Count;

			public bool IsReadOnly => throw new NotImplementedException();

			public void Add (T item) => m_Source.Add(item);

			public void Clear () => m_Source.Clear();

			public bool Contains (T item) => m_Source.Contains(item);

			public void CopyTo (T[] array,int arrayIndex) => m_Source.CopyTo(array,arrayIndex);

			public IEnumerator<T> GetEnumerator () => m_Source.GetEnumerator();

			public bool Remove (T item) => m_Source.Remove(item);

			IEnumerator IEnumerable.GetEnumerator () => m_Source.GetEnumerator();

		}

		class ReadOnlyCollection<T> : IReadOnlyCollection<T> {

			readonly List<T> m_Source;

			public ReadOnlyCollection (List<T> source) {
				m_Source = source;
			}

			public int Count => m_Source.Count;

			public IEnumerator<T> GetEnumerator () => m_Source.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator () => m_Source.GetEnumerator();
		}
	}
}