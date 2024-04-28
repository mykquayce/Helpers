using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(NonParallelCollectionDefinitionClass))]
public class MathTests
{
	[Theory]
	[InlineData(143, 7_000)]
	[InlineData(344, 2_900)]
	public void KelvinsConversionTests(int before, int expected)
	{
		var actual = before.ConvertFromElgatoToKelvin();
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(7_000, 143)]
	[InlineData(2_900, 344)]
	public void ElgatoConversionTests(int before, int expected)
	{
		var actual = ((short)before).ConvertFromKelvinToElgato();
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(143, 344, 143, 0d)]
	[InlineData(143, 344, 244, 0.5d)]
	[InlineData(143, 344, 344, 1d)]
	[InlineData(344, 143, 143, 1d)]
	[InlineData(344, 143, 244, 0.5d)]
	[InlineData(344, 143, 344, 0d)]
	public void ReduceToFraction(int min, int max, int before, double expected)
	{
		var range = new Range(min, max);
		var actual = range.ReduceValueToFraction(before);
		Assert.Equal(expected, actual, precision: 2);
	}

	[Theory]
	[InlineData(143, 344, 0d, 143)]
	[InlineData(143, 344, 0.5d, 243)]
	[InlineData(143, 344, 1d, 344)]
	[InlineData(344, 143, 0d, 344)]
	[InlineData(344, 143, 0.5d, 244)]
	[InlineData(344, 143, 1d, 143)]
	public void IncreaseFromFractionTests(int min, int max, double before, int expected)
	{
		var range = new Range(min, max);
		var actual = range.IncreaseValueFromFraction(before);
		Assert.Equal(expected, actual);
	}
}
