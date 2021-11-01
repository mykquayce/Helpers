using Dawn;
using Xunit;

namespace Helpers.MySql.Tests;

public class ConfigTests
{
	[Theory]
	[InlineData("hello", default, "hello")]
	[InlineData(default, "./Data/password.txt", "hello")]
	public void Test1(string? password, string? passwordFile, string expected)
	{
		var config = new Config("localhost", 3_306, "database", "userId", Password: password, PasswordFile: passwordFile);

		var connectionString = config.ConnectionString;

		var kvps = connectionString.ToDictionary();

		Assert.NotNull(kvps["password"]);
		Assert.NotEmpty(kvps["password"]);
		Assert.Equal(expected, kvps["password"]);
	}

	[Theory]
	[InlineData(default)]
	public void DawnGuardTests_NullStringsPassNotEmptyAndNotWhiteSpace(string? s)
	{
		Guard.Argument(s).NotEmpty();
		Guard.Argument(s).NotWhiteSpace();
	}
}
