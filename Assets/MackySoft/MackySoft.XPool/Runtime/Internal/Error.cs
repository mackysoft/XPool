using System;
using System.Runtime.CompilerServices;

namespace MackySoft.XPool.Internal {
    internal static class Error {

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentNullException ArgumentNullException (string paramName) {
			return new ArgumentNullException(paramName);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InvalidOperationException Empty () {
			return new InvalidOperationException("Collection is empty.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentOutOfRangeException ArgumentOutOfRangeOfCollection (string paramName) {
			return new ArgumentOutOfRangeException(paramName,"Parameter is out of range of collection.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentOutOfRangeException RequiredNonNegative (string paramName) {
			return new ArgumentOutOfRangeException(paramName,"Must be a non-negative value.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NullReferenceException FactoryMustReturnNotNull () {
			return new NullReferenceException("Factory method must return not null.");
		}

	}
}