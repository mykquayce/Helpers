using Xunit;

namespace Helpers.Elgato.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
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
	[InlineData(143, 344, 143, 0d)]
	[InlineData(143, 344, 244, 0.5d)]
	[InlineData(143, 344, 344, 1d)]
	public void ReduceToFraction(int min, int max, int before, double expected)
	{
		var range = new Range(min, max);
		var actual = range.ReduceValueToFraction(before);
		Assert.Equal(expected, actual, precision: 2);
	}

	[Theory]
	[InlineData(0d, 1d)]
	[InlineData(1d, 0d)]
	[InlineData(.5d, .5d)]
	[InlineData(.25d, .75d)]
	[InlineData(.75d, .25d)]
	public void InversionTests(double before, double expected)
	{
		var actual = before.Invert();
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(143, 344, 0d, 143)]
	[InlineData(143, 344, 0.5d, 243)]
	[InlineData(143, 344, 1d, 344)]
	public void IncreaseFromFractionTests(int min, int max, double before, int expected)
	{
		var range = new Range(min, max);
		var actual = range.IncreaseValueFromFraction(before);
		Assert.Equal(expected, actual);
	}
}
