namespace Helpers.NetworkDiscovery.Tests;

public class ClientTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IClient _sut;

	public ClientTests(Fixtures.Fixture fixture)
	{
		_sut = fixture.Client;
	}

	[Fact]
	public async Task AliasesTests()
	{
		var aliases = await _sut.GetAliasesAsync().ToArrayAsync();

		Assert.NotEmpty(aliases);
		Assert.All(aliases, Assert.NotNull);
		Assert.All(aliases, Assert.NotEmpty);
	}

	[Fact]
	public Task ResetTests() => _sut.ResetAsync();

	[Theory]
	[InlineData("28ee52eb0aa4")]
	[InlineData("vr front")]
	public async Task ResolveTests(object key)
	{
		var now = DateTime.UtcNow;
		var lease = await _sut.ResolveAsync(key);

		Assert.NotNull(lease);
		Assert.NotEqual(default, lease);
		Assert.InRange(lease.Expiration, now, now.AddDays(1));
	}
}
