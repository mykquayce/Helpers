namespace Helpers.GlobalCache.Tests;

[Collection(nameof(NonParallelCollection))]
public class ClientTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly IClient _sut;

	public ClientTests(Fixtures.ClientFixture fixture)
	{
		_sut = fixture.Client;
	}

	[Theory]
	// amp-mute-toggle
	[InlineData(
		"sendir,1:1,1,40192,2,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
		"completeir")]
	// amp-power-toggle
	[InlineData(
		"sendir,1:1,1,40192,2,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r",
		"completeir")]
	public async Task SendTests(string message, string expected)
	{
		var response = await _sut.SendAsync(message);
		Assert.NotNull(response);
		Assert.StartsWith(expected, response, StringComparison.OrdinalIgnoreCase);
	}
}
