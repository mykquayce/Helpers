using System.Drawing;

namespace Helpers.PhilipsHue.Tests;

public class DrawingExtensionsTests
{
	[Theory]
	[InlineData(.3501f, .1939f, 64, "ff3c2440")]
	[InlineData(.5356f, .3286f, 64, "ff40221b")]
	public void ToColorTests(float x, float y, byte bri, string expected)
	{
		var point = new PointF(x: x, y: y);
		var actual = point.ToColor(bri);
		Assert.Equal(expected, actual.Name);
	}

	[Theory]
	[InlineData(0xf2, 0x90, 0xff)]
	[InlineData(0xff, 0x8a, 0x6c)]
	[InlineData(0x77, 0, 0)]
	public void ToXYTests(params int[] bytes)
	{
		var color = Color.FromArgb(bytes[0], bytes[1], bytes[2]);
		var (point, bri) = color.ToXY();
	}
}
