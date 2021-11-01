using Xunit;

namespace Helpers.MySql.Tests;

public class SafeDictionaryTests
{
	[Theory]
	[InlineData("key", 0)]
	public void GetNew_AddsKey(string key, int defaultValue)
	{
		IDictionary<string, int> sut = new SafeDictionary<string, int>(defaultValue);

		sut[key]++;

		var actual = sut[key];

		Assert.Equal(1, actual);
	}
}
