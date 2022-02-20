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

	}
}