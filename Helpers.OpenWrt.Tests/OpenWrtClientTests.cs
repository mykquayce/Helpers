using Xunit;

namespace Helpers.OpenWrt.Tests;

public sealed class OpenWrtClientTests : IClassFixture<Fixtures.OpenWrtClientFixture>
{
	private readonly Clients.IOpenWrtClient _sut;

	public OpenWrtClientTests(Fixtures.OpenWrtClientFixture fixture)
	{
		_sut = fixture.OpenWrtClient;
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
