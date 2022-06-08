namespace System;

public static class SystemExtensions
{
	private readonly static Range _elgatoRange = new(143, 344);
	private readonly static Range _kelvinRange = new(7_000, 2_900);

	public static void Deconstruct(this Range range, out int start, out int end)
	{
		start = range.Start.Value;
		end = range.End.Value;
	}

	public static int Round(this float f) => (int)Math.Round(f);
	public static int Round(this double d) => (int)Math.Round(d);

	public static double ReduceValueToFraction(this Range range, int value)
	{
		var (min, max) = range;
		return (value - min) / (double)(max - min);
	}

	public static int IncreaseValueFromFraction(this Range range, double value)
	{
		var (min, max) = range;
		return ((max - min) * value).Round() + min;
	}

	public static short ConvertFromElgatoToKelvin(this int elgato)
	{
		var fraction = _elgatoRange.ReduceValueToFraction(elgato);
		return (short)_kelvinRange.IncreaseValueFromFraction(fraction);
	}

	public static short ConvertFromKelvinToElgato(this short kelvin)
	{
		var fraction = _kelvinRange.ReduceValueToFraction(kelvin);
		return (short)_elgatoRange.IncreaseValueFromFraction(fraction);
	}
}
