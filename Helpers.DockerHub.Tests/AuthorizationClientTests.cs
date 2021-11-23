using Xunit;

namespace Helpers.DockerHub.Tests;

public class AuthorizationClientTests : IClassFixture<Fixtures.AuthorizationClientFixture>
{
	private readonly IAuthorizationClient _sut;

	public AuthorizationClientTests(Fixtures.AuthorizationClientFixture fixture)
	{
		_sut = fixture.AuthorizationClient;
	}

	[Theory]
	[InlineData("pihole", "pihole")]
	public async Task GetTokenFromRemoteTests(string organization, string repository)
	{
		var now = DateTime.UtcNow;
		var (token, expiry) = await _sut.GetTokenFromRemoteAsync(organization, repository);

		Assert.NotNull(token);
		Assert.Matches(@"^[0-9A-Za-z]{1000,}\.[0-9A-Za-z]{300,}\.[0-9A-Za-z-_]{300,}$", token);
		Assert.InRange(expiry, now.AddMinutes(5), now.AddMinutes(5.1));
	}

	[Theory]
	[InlineData(10, "pihole", "pihole")]
	public async Task CacheTests(int count, string organization, string repository)
	{
		// Get one (should be cached)
		var first = await _sut.GetTokenAsync(organization, repository);

		// Get ten (should be the same)
		while (--count >= 0)
		{
			var another = await _sut.GetTokenAsync(organization, repository);
			Assert.Equal(first, another);
		}
	}
}
