using System.Numerics;

namespace System;

public static class SystemExtensions
{
	public static IEnumerable<int> GetIndices(this string s, string chars, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
	{
		foreach (var c in s)
		{
			yield return chars.IndexOf(c, comparison);
		}
	}

	public static IEnumerable<T> GetPowers<T>(this T value, int @base)
		where T : INumber<T>
	{
		if (value == T.Zero)
		{
			yield return T.Zero;
			yield break;
		}

		T b = T.CreateChecked(@base);

		while (value > T.Zero)
		{
			yield return value % b;
			value /= b;
		}
	}
}
