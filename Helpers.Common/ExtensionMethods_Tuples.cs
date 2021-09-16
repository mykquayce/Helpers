using Dawn;
using System.Collections.Generic;

namespace Helpers.Common
{
	public static partial class ExtensionMethods
	{
		public static IEnumerator<(T, T)> GetEnumerator<T>(this (IEnumerable<T>, IEnumerable<T>) tuple)
		{
			Guard.Argument(tuple).NotDefault();
			var first = Guard.Argument(tuple.Item1).NotNull().Wrap(e => e.GetEnumerator()).NotNull().Value;
			var second = Guard.Argument(tuple.Item2).NotNull().Wrap(e => e.GetEnumerator()).NotNull().Value;

			return new DoubleEnumerator<T>(first, second);
		}
	}
}
