using System;
using System.Collections;
using System.Collections.Generic;
using MackySoft.XPool.Collections.Tests;
using NUnit.Framework;
using UnityEngine;

namespace MackySoft.XPool.Tests {
	public class FactoryPoolTest {
		
		[Test]
		public void Throw_if_capcity_less_than_or_equal_to_zero () {
			Assert.Throws<ArgumentOutOfRangeException>(() => new FactoryPool<Unit>(0,() => Unit.Default,null,null));
			Assert.Throws<ArgumentOutOfRangeException>(() => new FactoryPool<Unit>(-1,() => Unit.Default,null,null));
		}

	}
}