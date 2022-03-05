namespace System.Collections.Generic;

public static class EnumerableMethods
{
	public static bool Overlaps<T>(this IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
		=> left.Join(right, l => l, r => r, (l, r) => l, comparer).Any();
}
