using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Helpers.PhilipsHue.Tests.Comparers;

public class ColorComparer : IEqualityComparer<Color>
{
	private readonly double _tolerance;

	public ColorComparer(double tolerance)
	{
		_tolerance = tolerance;
	}

	public bool Equals(Color x, Color y)
	{
		var max = byte.MaxValue * _tolerance;

		return Math.Abs(x.A - y.A) < max
			&& Math.Abs(x.R - y.R) < max
			&& Math.Abs(x.G - y.G) < max
			&& Math.Abs(x.B - y.B) < max;
	}

	public int GetHashCode([DisallowNull] Color obj) => obj.GetHashCode();

	public static ColorComparer OnePercentTolerance => new(.01);
	public static ColorComparer TwoPercentTolerance => new(.02);
	public static ColorComparer ThreePercentTolerance => new(.03);
}
