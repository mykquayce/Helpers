using Xunit;

namespace Helpers.SSH.Tests;

public class ClientTests(Fixtures.Fixture fixture) : IClassFixture<Fixtures.Fixture>
{
	private readonly IClient _sut = fixture.Client;

	[Theory]
	[InlineData("date")]
	[InlineData("cat /tmp/dhcp.leases")]
	public async Task RunCommandTests(string commandText)
	{
		var output = await _sut.RunCommandAsync(commandText);

		Assert.NotEmpty(output);
	}
}
