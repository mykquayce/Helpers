namespace Helpers.NetworkDiscovery.Tests;

public class ClientTests(Fixtures.Fixture fixture) : IClassFixture<Fixtures.Fixture>
{
	private readonly IClient _sut = fixture.Client;

	[Fact]
	public async Task AllLeasesTests()
	{
		var aliases = await _sut.GetAllLeasesAsync().ToArrayAsync();

		Assert.NotEmpty(aliases);
		Assert.All(aliases, Assert.NotNull);
	}

	[Fact]
	public Task ResetTests() => _sut.ResetAsync();

	[Theory]
	[InlineData("000c1e059cad")]
	[InlineData("itach059cad")]
	public async Task ResolveTests(object key)
	{
		var now = DateTime.UtcNow;
		var lease = await _sut.ResolveAsync(key);

		Assert.NotNull(lease);
		Assert.NotEqual(default, lease);
		Assert.InRange(lease.Expiration, now, now.AddDays(1));
	}
}
