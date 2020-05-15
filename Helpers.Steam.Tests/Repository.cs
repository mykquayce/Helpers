using Helpers.MySql.Models;
using Helpers.Steam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers.Steam.Tests
{
	public class Repository : Helpers.MySql.RepositoryBase
	{
		public Repository(DbSettings dbSettings)
			: base(dbSettings)
		{ }

		public Task<int> SaveUserAsync(long id, string name)
		{
			return base.ExecuteAsync(
				sql: "INSERT IGNORE INTO `steam`.`user` (`id`, `name`) VALUES (@id, @name);",
				param: new {id, name, });
		}

		public IAsyncEnumerable<long> GetUserIdsAsync()
			=> base.QueryAsync<long>("SELECT `id` FROM `steam`.`user`;");


		public IAsyncEnumerable<int> GetAppIdsAsync()
			=> base.QueryAsync<int>("select `id` from `steam`.`app` WHERE `id`>=255220;");

		public async Task SaveCategoriesAsync(IDictionary<int, string> categories, int appId)
		{
			using var transaction = base.BeginTransaction();

			await base.ExecuteAsync(
				sql: "INSERT IGNORE INTO `steam`.`category` (`id`, `name`) VALUES (@Key, @Value);",
				param: from kvp in categories
					   select new { kvp.Key, kvp.Value, },
				transaction: transaction);

			await base.ExecuteAsync(
				sql: "INSERT IGNORE INTO `steam`.`appcategory` (`appId`, `categoryId`) VALUES (@appId, @Key);",
				param: from kvp in categories
					   select new { appId, kvp.Key, },
				transaction: transaction);

			transaction.Commit();
		}

		public async Task SaveGamesAsync(IEnumerable<Game> games, long userId)
		{
			using var transaction = base.BeginTransaction();

			await base.ExecuteAsync(
				sql: "INSERT IGNORE INTO `steam`.`app` (`id`, `name`) VALUES (@AppId, @Name);",
				param: from g in games
					   select new { g.AppId, g.Name, },
				transaction: transaction);

			await base.ExecuteAsync(
				sql: "INSERT IGNORE INTO `steam`.`userapp` (`appId`, `userId`, `hours`) VALUES (@AppId, @userId, @Hours);",
				param: from g in games
					   let hours = MinutesToHours(g.Minutes ?? 0)
					   select new { g.AppId, userId, hours},
				transaction: transaction);

			transaction.Commit();
		}

		private static int MinutesToHours(int minutes) => RoundingAwayFromzero(minutes, i => (int)Math.Round(i / 60d));

		private static int RoundingAwayFromzero(int before, Func<int, int> func)
		{
			var after = func(before);

			if(before > 0 && after == 0)
			{
				return after + 1;
			}

			return after;
		}
	}
}
