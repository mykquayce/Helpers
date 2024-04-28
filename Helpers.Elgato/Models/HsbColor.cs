namespace System.Drawing;

/// <summary>
/// the values from System.Drawing.Color.GetHue(), .GetSaturation(), and .GetBrightness()
/// </summary>
/// <param name="Hue">float [0..360]</param>
/// <param name="Saturation">float [0..1]</param>
/// <param name="Brightness">float [0..1]</param>
public readonly record struct HsbColor(float Hue, float Saturation, float Brightness);
