using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Helpers.Elgato.Tests.Comparers;

public class TolerantEqualityComparer<T> : IEqualityComparer<T>
	where T : INumber<T>
{
	private readonly T _tolerance;

	private TolerantEqualityComparer(T tolerance)
	{
		_tolerance = tolerance;
	}

	public bool Equals(T? x, T? y) => T.Abs(x! - y!) <= _tolerance;

	public int GetHashCode([DisallowNull] T obj) => obj?.GetHashCode() ?? 0;

	public static TolerantEqualityComparer<T> Zero => new(T.Zero);
	public static TolerantEqualityComparer<T> One => new(T.One);
	public static TolerantEqualityComparer<T> Two => new(T.Parse("2", CultureInfo.InvariantCulture));
	public static TolerantEqualityComparer<T> Ten => new(T.Parse("10", CultureInfo.InvariantCulture));
}
