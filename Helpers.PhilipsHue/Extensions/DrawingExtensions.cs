namespace System.Drawing;

public static class DrawingExtensions
{
	public static void Deconstruct(this PointF point, out float x, out float y)
	{
		x = point.X;
		y = point.Y;
	}

	public static (PointF, byte) ToXY(this Color color)
	{
		var red = (double)color.R / byte.MaxValue;
		var green = (double)color.G / byte.MaxValue;
		var blue = (double)color.B / byte.MaxValue;

		var r = f(red);
		var g = f(green);
		var b = f(blue);

		var x = (r * .649926) + (g * .103455) + (b * .197109);
		var y = (r * .234327) + (g * .743075) + (b * .022598);
		var z = (r * 0) + (g * .053077) + (b * 1.035763);
		var sum = x + y + z;

		var point = new PointF(x: float.CreateSaturating(x / sum), y: float.CreateSaturating(y / sum));
		// maximum of red, green, and blue
		var bri = Math.Max(Math.Max(color.R, color.G), color.B);
		return (point, bri);

		static double f(double d)
		{
			if (d > .04045)
			{
				return Math.Pow((d + .055) / (1 + .055), 2.4);
			}

			return d / 12.92;
		}
	}

	public static Color ToColor(this PointF point, byte bri)
	{
		var z = (1d - point.X) - point.Y;
		var y = bri / 255d;
		var x = (y / point.Y) * point.X;
		z *= y / point.Y;
		var r = (x * 1.612) - (y * .203) - (z * .302);
		var g = (-x * .509) + (y * 1.412) + (z * .066);
		var b = (x * .026) - (y * .072) + (z * .962);
		r = f(r);
		g = f(g);
		b = f(b);
		var maxValue = Math.Max(Math.Max(r, g), b);
		r /= maxValue;
		g /= maxValue;
		b /= maxValue;

		// adjust for brightness
		r *= y;
		g *= y;
		b *= y;

		return Color.FromArgb(
			red: byte.CreateSaturating(r * 255),
			green: byte.CreateSaturating(g * 255),
			blue: byte.CreateSaturating(b * 255));

		static double f(double d) => d <= .0031308 ? 12.92 * d : (1 + .055) * (Math.Pow(d, 1 / 2.4) - .055);
	}
}
