using Dawn;

namespace System.Drawing;

/// <summary>
/// the values from System.Drawing.Color.GetHue(), .GetSaturation(), and .GetBrightness()
/// </summary>
/// <param name="Hue">float [0..360]</param>
/// <param name="Saturation">float [0..1]</param>
/// <param name="Brightness">float [0..1]</param>
public record HsbColor
{
	public HsbColor(float hue, float saturation, float brightness)
	{
		Hue = Guard.Argument(hue).InRange(0, 360).Value;
		Saturation = Guard.Argument(saturation).InRange(0, 1).Value;
		Brightness = Guard.Argument(brightness).InRange(0, 1).Value;
	}

	public float Hue { get; }
	public float Saturation { get; }
	public float Brightness { get; }

	public void Deconstruct(out float hue, out float saturation, out float brightness)
	{
		hue = Hue;
		saturation = Saturation;
		brightness = Brightness;
	}
}
