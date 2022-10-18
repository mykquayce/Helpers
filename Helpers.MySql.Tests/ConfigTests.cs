using Xunit;

namespace Helpers.MySql.Tests;

public class ConfigTests
{
	[Theory]
	[InlineData("satrsatrasrtratarts", 37_862, "fqwpfqwpfqpwqfpw", "glujgljulgujglju", "dnhnhdedhen", true)]
	[InlineData("localhost", 3_306, "database", "root", "root", false)]
	public void ConfigParseTests(string server, uint port, string database, string userId, string password, bool secure)
	{
		var left = new Config(server, port, database, userId, password, secure);
		var right = Config.Parse(left.ConnectionString, null);

		Assert.Equal(left.Server, right.Server, StringComparer.OrdinalIgnoreCase);
		Assert.Equal(left.Port, right.Port);
		Assert.Equal(left.Database, right.Database, StringComparer.OrdinalIgnoreCase);
		Assert.Equal(left.UserId, right.UserId, StringComparer.OrdinalIgnoreCase);
		Assert.Equal(left.Password, right.Password, StringComparer.OrdinalIgnoreCase);
		Assert.Equal(left.Secure, right.Secure);
	}
}
