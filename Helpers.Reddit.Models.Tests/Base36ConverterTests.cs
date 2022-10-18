using Helpers.Reddit.Models.Converters;
using Xunit;

namespace Helpers.Reddit.Models.Tests;

public class Base36ConverterTests
{
	[Theory]
	[InlineData("0", 0u)]
	[InlineData("1", 1u)]
	[InlineData("z", 35u)]
	[InlineData("10", 36u)]
	[InlineData("cm3ryv", 762_721_879)]
	[InlineData("g6kujao", 35_226_331_440)]
	[InlineData("nno9bs", 1_430_485_048u)]
	[InlineData("nnpqsp", 1_430_554_345u)]
	[InlineData("nwy8uj", 1_446_067_531u)]
	[InlineData("ptvaka", 1_561_823_290)]
	[InlineData("zzzzzz", 2_176_782_335u)]
	public void FromStringTests(string s, long expectedId)
	{
		var actualId = Base36Converter.FromString<long>(s);
		Assert.Equal(expectedId, actualId);
	}


	[Theory]
	[InlineData(0u, "0")]
	[InlineData(1u, "1")]
	[InlineData(35u, "z")]
	[InlineData(36u, "10")]
	[InlineData(35_226_331_440, "g6kujao")]
	[InlineData(1_430_485_048u, "nno9bs")]
	[InlineData(1_430_554_345u, "nnpqsp")]
	[InlineData(1_446_067_531u, "nwy8uj")]
	[InlineData(762_721_879, "cm3ryv")]
	[InlineData(1_561_823_290, "ptvaka")]
	[InlineData(2_176_782_335u, "zzzzzz")]
	public void ToStringTests(long id, string expected)
	{
		var actual = Base36Converter.ToString(id);
		Assert.Equal(expected, actual);
	}
}
