namespace System.Drawing;

public static class DrawingExtensions
{
	public static HsbColor GetHsbColor(this Color color)
	{
		return new(
			hue: color.GetHue(),
			saturation: color.GetSaturation(),
			brightness: color.GetBrightness());
	}

	public static Color GetColor(this HsbColor hsbColor)
	{
		var (hue, saturation, brightness) = hsbColor;

		if (saturation == 0)
		{
			var num = (byte)Math.Round(brightness * 255);
			return Color.FromArgb(num, num, num);
		}

		var q = brightness < .5
			? brightness * (1 + saturation)
			: brightness + saturation - brightness * saturation;
		var p = 2 * brightness - q;
		var Hk = hue / 360;

		var red = func(Hk + 1d / 3);
		var green = func(Hk);
		var blue = func(Hk - 1d / 3);

		return Color.FromArgb(
			red: (int)Math.Round(red * 255),
			green: (int)Math.Round(green * 255),
			blue: (int)Math.Round(blue * 255));

		double func(double t)
		{
			while (t < 0) t++;
			while (t > 1) t--;

			return (t * 6) switch
			{
				< 1 => p + (q - p) * 6 * t,
				< 3 => q,
				< 4 => p + (q - p) * (2d / 3 - t) * 6,
				_ => p,
			};
		}
	}
}
