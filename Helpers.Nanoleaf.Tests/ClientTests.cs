using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Helpers.Nanoleaf.Tests;

public class ClientTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly IClient _sut = fixture.Client;

	[SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "test requires hardware interacttion")]
	[Fact(Skip = "requires hardware interacttion (holding the power button for 5s)")]
	public async Task GetTokenTests()
	{
		string token;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 10_000);
			token = await _sut.GetTokenAsync(cts.Token);
		}
		Assert.NotEmpty(token);
		Assert.Matches(@"^[\d\w]{32}$", token);
	}

	[Fact]
	public async Task GetInfoTests()
	{
		var info = await _sut.GetInfoAsync();
		Assert.StartsWith("Shapes ", info.name, StringComparison.OrdinalIgnoreCase);
	}

	[Theory]
	[InlineData("Cocoa Beach")]
	public async Task SetEffectTests(string effect)
	{
		var response = await _sut.SetEffectAsync(effect);
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Theory, InlineData(true)]
	public async Task SetOnTests(bool value)
	{
		var response = await _sut.SetOnAsync(value);
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}
}
