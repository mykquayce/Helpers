namespace Helpers.Identity.Tests;

public class IdentityClientTests(Fixture fixture) : IClassFixture<Fixture>
{
	private readonly Clients.IIdentityClient _sut = fixture.IdentityClient;

	[Fact]
	public async Task TokenTests()
	{
		// Act
		var token = await _sut.GetAccessTokenAsync();

		// Assert
		Assert.NotEmpty(token);
		Assert.StartsWith("eyJ", token);
	}

	[Theory, InlineData(10)]
	public async Task CachingTests(int count)
	{
		// Arrange
		var tokens = new List<string>(capacity: count);

		// Act
		while (count-- > 0)
		{
			var token = await _sut.GetAccessTokenAsync();
			tokens.Add(token);
		}

		// Assert
		Assert.DoesNotContain(default, tokens);
		Assert.Single(tokens.Distinct());
	}
}
