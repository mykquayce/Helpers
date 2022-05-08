using Xunit;

namespace Helpers.OpenWrt.Tests;

public sealed class OpenWrtClientTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly IClient _sut;

	public OpenWrtClientTests(Fixtures.ClientFixture fixture)
	{
		_sut = fixture.Client;
	}

	[Theory]
	[InlineData("echo -n 'hello world'", "^hello world$")]
	[InlineData("ip route show", @"(\d+\.\d+\.\d+\.\d+)")]
	public async Task ExecuteCommand(string command, string expected)
	{
		var actual = await _sut.ExecuteCommandAsync(command);
		Assert.Matches(expected, actual);
	}
}
