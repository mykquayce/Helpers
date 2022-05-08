using System.Drawing;

namespace Helpers.Elgato.Models;

public record RgbLightObject(bool On, float Brightness, byte Red, byte Green, byte Blue)
	: LightObject(On, Brightness)
{
	public static explicit operator Generated.LightObject(RgbLightObject light)
	{
		int on = light.On ? 1 : 0;
		int brightness = (int)Math.Round(light.Brightness * 100f);
		Color color = Color.FromArgb(red: light.Red, green: light.Green, blue: light.Blue);
		HsbColor hsbColor = color.GetHsbColor();
		double hue = hsbColor.Hue;
		double saturation = (int)Math.Round(hsbColor.Saturation * 100f);

		return new(on: on, brightness: brightness, temperature: null, hue: hue, saturation: saturation);
	}
}
