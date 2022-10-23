using Dawn;
using Helpers.Steam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
			_steamClient = Guard.Argument(steamClientFixture).NotNull()
				.Wrap(f => f.SteamClient).NotNull().Value;

			_steamIds = Guard.Argument(userSecretsFixture).NotNull()
				.Wrap(f => f.SteamIds!).NotNull().NotEmpty().DoesNotContainNull().DoesNotContainDuplicate().Value;
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
			var dictionary = new Dictionary<string, short>();

			foreach (var steamId in _steamIds)
			{
				var games = _steamClient.GetOwnedGamesAsync(steamId);

				await foreach (var game in games)
				{
					var key = game.Name ?? throw new ArgumentOutOfRangeException();

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
		[InlineData(203160)]
		[InlineData(218620)]
		[InlineData(221680)]
		[InlineData(221910)]
		[InlineData(271590)]
		[InlineData(550)]
		[InlineData(72850)]
		public async Task GetApplicationDetailsTest(int appId)
		{
			var details = await _steamClient.GetAppDetailsAsync(appId);

			Assert.NotNull(details);
			Assert.NotNull(details.Categories);
			Assert.NotEmpty(details.Categories);
		}

		[Theory]
		[InlineData(43110)]
		[InlineData(4540)]
		public async Task GetAppDetailsAsync_Failing(int appId)
		{
			try
			{
				await _steamClient.GetAppDetailsAsync(appId);
				Assert.True(false);
			}
			catch (Exception ex)
			{
				Assert.IsType<Exceptions.AppNotFoundException>(ex);

				var appNotFoundException = (Exceptions.AppNotFoundException)ex;

				Assert.Equal(appId, appNotFoundException.AppId);
				Assert.Equal($@"App {appId:D} not found", appNotFoundException.Message);
				Assert.NotNull(appNotFoundException.Data);
				Assert.NotEmpty(appNotFoundException.Data);
				Assert.NotNull(appNotFoundException.Data[nameof(appId)]);
				Assert.Equal(appId, appNotFoundException.Data[nameof(appId)]);
			}
		}

		[Fact]
		public async Task GetCategories()
		{
			var dictionary = new Dictionary<int, string?>();

			foreach (var steamId in _steamIds)
			{
				var games = _steamClient.GetOwnedGamesAsync(steamId);

				await foreach (var game in games)
				{
					if (!game.AppId.HasValue) continue;

					var appId = game.AppId!.Value;

					AppDetails app;

					try { app = await _steamClient.GetAppDetailsAsync(appId); }
					catch (Exceptions.AppNotFoundException) { continue; }

					if (app.Categories_Array is null) continue;

					foreach (var category in app.Categories_Array)
					{
						var key = category.Id
							?? throw new ArgumentOutOfRangeException(nameof(AppDetails.Category.Id));

						var value = category.Description
							?? throw new ArgumentOutOfRangeException(nameof(AppDetails.Category.Description));

						dictionary.TryAdd(key, value);
					}
				}
			}
		}

		[Fact(Timeout = 600_000)]
		public async Task TooManyRequests()
		{
			var count = 0;

			using var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			using var client = new System.Net.Http.HttpClient(handler);

			foreach (var steamId in _steamIds)
			{
				await foreach (var game in _steamClient.GetOwnedGamesAsync(steamId))
				{
					count++;
					var appId = game.AppId!.Value;

					try { await _steamClient.GetAppDetailsAsync(appId); }
					catch (Exceptions.AppNotFoundException) { }
				}
			}
		}
	}
}
