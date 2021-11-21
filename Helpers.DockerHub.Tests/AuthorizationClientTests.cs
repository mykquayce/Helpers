using Xunit;

namespace Helpers.DockerHub.Tests;

public class AuthorizationClientTests : IClassFixture<Fixtures.AuthorizationClientFixture>
{
	private readonly IAuthorizationClient _sut;

	public AuthorizationClientTests(Fixtures.AuthorizationClientFixture fixture)
	{
		_sut = fixture.AuthorizationClient;
	}

	[Fact]
	public async Task GetTokenFromRemoteTests()
	{
		var now = DateTime.UtcNow;
		var (token, expiry) = await _sut.GetTokenFromRemoteAsync();

		Assert.NotNull(token);
		Assert.Matches(@"^[0-9A-Za-z]{1000,}\.[0-9A-Za-z]{300,}\.[0-9A-Za-z-_]{300,}$", token);
		Assert.InRange(expiry, now.AddMinutes(5), now.AddMinutes(5.1));
	}

	[Theory]
	[InlineData(10)]
	public async Task CacheTests(int count)
	{
		// Get one (should be cached)
		var first = await _sut.GetTokenAsync();

		// Get ten (should be the same)
		while (--count >= 0)
		{
			var another = await _sut.GetTokenAsync();
			Assert.Equal(first, another);
		}
	}
}
