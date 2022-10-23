using Helpers.Steam.Exceptions;
using Helpers.Steam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Steam.Tests
{
	public class BuildDatabase : IClassFixture<Fixtures.UserSecretsFixture>
	{
		private readonly Helpers.Steam.ISteamClient _steamClient;
		private readonly Repository _repository;

		public BuildDatabase(Fixtures.UserSecretsFixture userSecretsFixture)
		{
			var key = userSecretsFixture.SteamKey;
			var dbSettings = userSecretsFixture.DbSettings;

			_steamClient = new Helpers.Steam.Concrete.SteamClient(key);

			_repository = new Repository(dbSettings!);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "destructive")]
		[Theory(Skip = "not really a unit test: just a way to populate the database")]
		[InlineData(76561197998128899, "platypusrcool")]
		[InlineData(76561198033583828, "andreas3115")]
		[InlineData(76561197965007048, "StealthBanana")]
		[InlineData(76561198045851424, "Delphboy")]
		[InlineData(76561197960975794, "pablocampy")]
		[InlineData(76561198025438682, "chipbarm")]
		[InlineData(76561198011300030, "steviedisco")]

		public Task SaveUserAsync(long id, string name)
		{
			return _repository.SaveUserAsync(id, name);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "destructive")]
		[Fact(Skip = "not really a unit test: just a way to populate the database")]
		public async Task GetAppsForUsers()
		{
			await foreach (var userId in _repository.GetUserIdsAsync())
			{
				var games = await _steamClient.GetOwnedGamesAsync(userId).ToListAsync();

				await _repository.SaveGamesAsync(games, userId);
			}
		}

		[Fact(Timeout = 3_600_000)]
		public async Task GetCategoriesForApps()
		{
			await foreach (var appId in _repository.GetAppIdsAsync())
			{
				AppDetails details;

				try { details = await _steamClient.GetAppDetailsAsync(appId); }
				catch (AppNotFoundException) { continue; }

				var categories = details?.Categories_Array?
					.ToDictionary(a => a.Id!.Value, a => a.Description!);

				if (categories?.Any() ?? false)
				{
					await _repository.SaveCategoriesAsync(categories, appId);
				}
			}
		}
	}
}
