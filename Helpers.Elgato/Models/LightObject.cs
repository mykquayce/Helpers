using System.Drawing;

namespace Helpers.Elgato.Models;

public abstract record LightObject(bool On, float Brightness)
{
	public static explicit operator LightObject(Generated.LightObject light)
	{
		bool on = light.on == 1;
		float brightness = light.brightness / 100f;

		if (light.temperature is not null)
		{
			short kelvins = Convert.TemperatureToKelvins(light.temperature!.Value);
			return new WhiteLightObject(On: on, Brightness: brightness, Kelvins: kelvins);
		}

		float hue = (float)light.hue!.Value;
		float saturation = (float)(light.saturation!.Value / 100d);
		HsbColor hsbColor = new(Hue: hue, Saturation: saturation, Brightness: brightness);
		Color color = hsbColor.GetColor();

		return new RgbLightObject(On: on, Brightness: brightness, Red: color.R, Green: color.G, Blue: color.B);
	}

	public static explicit operator Generated.LightObject(LightObject light)
	{
		return light switch
		{
			RgbLightObject rgb => (Generated.LightObject)rgb,
			WhiteLightObject white => (Generated.LightObject)white,
			_ => throw new ArgumentOutOfRangeException(nameof(light), light, $"unexpected {nameof(light)} {light}"),
		};
	}
}
