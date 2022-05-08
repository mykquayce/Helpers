namespace Helpers.Elgato.Models;

public record WhiteLightObject(bool On, float Brightness, short Kelvins)
	: LightObject(On, Brightness)
{
	public static explicit operator Generated.LightObject(WhiteLightObject light)
	{
		int on = light.On ? 1 : 0;
		int brightness = (int)Math.Round(light.Brightness * 100f);
		int temperature = Convert.KelvinsToTemperature(light.Kelvins);

		return new(on: on, brightness: brightness, temperature: temperature, hue: null, saturation: null);
	}
}
