namespace Helpers.Elgato;

public static class Convert
{
	public static short TemperatureToKelvins(int temperature)
	{
		var fraction = (temperature - 143) / 201d;
		var inverted = 1 - fraction;
		var kelvins = inverted * 4_100 + 2_900;
		return (short)Math.Round(kelvins);
	}

	public static int KelvinsToTemperature(short kelvins)
	{
		var fraction = (kelvins - 2_900) / 4_100d;
		var inverted = 1 - fraction;
		var temperature = inverted * 201 + 143;
		return (int)Math.Round(temperature);
	}
}
