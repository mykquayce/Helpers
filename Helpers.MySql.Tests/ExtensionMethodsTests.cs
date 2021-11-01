using Xunit;

namespace Helpers.MySql.Tests;

public class ExtensionMethodsTests
{
	[Theory]
	[InlineData(
		"server=localhost;port=3306;user id=root;password=xiebeiyoothohYaidieroh8ahchohphi;database=test;",
		"server", "localhost",
		"port", "3306",
		"user id", "root",
		"password", "xiebeiyoothohYaidieroh8ahchohphi",
		"database", "test")]
	public void ExtensionMethodsTests_ToDictionary_ContainsAllTheKeysAndValues(
		string before,
		params string[] expected)
	{
		// Act
		var actual = before.ToDictionary();

		// Assert
		for (var a = 0; a < expected.Length - 1; a += 2)
		{
			var key = expected[a];
			var value = expected[a + 1];

			Assert.Contains(key, actual.Keys);
			Assert.Equal(value, actual[key]);
		}
	}
}
