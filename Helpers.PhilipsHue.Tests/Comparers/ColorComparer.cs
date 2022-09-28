using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Helpers.PhilipsHue.Tests.Comparers;

public class ColorComparer : IEqualityComparer<Color>
{
	private readonly int _tolerance;

	public ColorComparer(int tolerance)
	{
		_tolerance = tolerance;
	}

	public bool Equals(Color x, Color y)
	{
		return Math.Abs(x.R - y.R) <= _tolerance
			&& Math.Abs(x.G - y.G) <= _tolerance
			&& Math.Abs(x.B - y.B) <= _tolerance;
	}

	public int GetHashCode([DisallowNull] Color obj) => obj.GetHashCode();
}
