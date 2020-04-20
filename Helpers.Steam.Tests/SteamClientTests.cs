using Dawn;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Steam.Tests
{
	public class SteamClientTests : IClassFixture<Fixtures.SteamClientFixture>, IClassFixture<Fixtures.UserSecretsFixture>
	{
		private readonly ISteamClient _steamClient;
		private readonly IReadOnlyList<long> _steamIds;

		public SteamClientTests(Fixtures.SteamClientFixture steamClientFixture, Fixtures.UserSecretsFixture userSecretsFixture)
		{
			_steamClient = Guard.Argument(() => steamClientFixture).NotNull()
				.Wrap(f => f.SteamClient).NotNull().Value;

			_steamIds = Guard.Argument(() => userSecretsFixture).NotNull()
				.Wrap(f => f.SteamIds).NotNull().NotEmpty().DoesNotContainNull().DoesNotContainDuplicate().Value;
		}


		[Fact]
		public async Task GetOwnedGamesTest()
		{
			var count = 0;

			await foreach (var game in _steamClient.GetOwnedGamesAsync(_steamIds[0]))
			{
				count++;
				Assert.NotNull(game!.AppId);
				Assert.InRange(game.AppId!.Value, 0, int.MaxValue);
				Assert.InRange(game.Minutes!.Value, 0, int.MaxValue);
				Assert.NotNull(game.Name);
				Assert.NotEmpty(game.Name);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		[Fact]
		public async Task Correlate()
		{
			var dictionary = new Dictionary<(int, string), short>();

			foreach (var steamId in _steamIds)
			{
				var games = _steamClient.GetOwnedGamesAsync(steamId);

				await foreach (var game in games)
				{
					var key = (game.AppId!.Value, game.Name);

					if (!dictionary.TryAdd(key, 1))
					{
						dictionary[key]++;
					}
				}
			}

			var filtered = (from kvp in dictionary
							where kvp.Value >= 4
							orderby kvp.Value descending, kvp.Key
							select kvp
						).ToList();

			Assert.NotEmpty(filtered);
		}

		[Theory]
		[InlineData(550)]
		[InlineData(43110)]
		[InlineData(72850)]
		[InlineData(203160)]
		[InlineData(218620)]
		[InlineData(221910)]
		[InlineData(271590)]
		public async Task GetApplicationDetailsTest(int appId)
		{

		}
	}
}
