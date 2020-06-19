using System;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Twitch.Tests
{
	public class TwitchClientTests : IClassFixture<Fixtures.UserSecretsFixture>, IDisposable
	{
		private readonly TwitchClient _sut;

		public TwitchClientTests(Fixtures.UserSecretsFixture fixture)
		{
			var config = new Models.Config
			{
				BearerToken = fixture.BearerToken,
				ClientId = fixture.ClientId,
				ClientSecret = fixture.ClientSecret,
			};

			_sut = new TwitchClient(config);
		}

		[Fact]
		public async Task GetTokenAsyncTests()
		{
			// Act
			var (token, expiry) = await _sut.GetTokenAsync();

			// Assert
			Assert.Matches("^[0-9a-z]{30}$", token);

			var now = DateTime.UtcNow;
			Assert.InRange(expiry, now.AddSeconds(1), now.AddYears(5));
		}

		[Theory]
		[InlineData("mykquayce")]
		public async Task GetUsersAsyncTests(params string[] logins)
		{
			// Arrange
			var count = 0;

			// Act
			var users = _sut.GetUsersAsync(logins);

			// Assert
			await foreach (var (id, login) in users)
			{
				count++;
				Assert.InRange(id, 1, int.MaxValue);
				Assert.Matches("^[_0-9a-z]+$", login);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		public void Dispose()
		{
			_sut?.Dispose();
		}
	}
}
