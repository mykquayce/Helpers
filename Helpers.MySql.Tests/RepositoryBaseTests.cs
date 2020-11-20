using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.MySql.Tests
{
	public class RepositoryBaseTests
	{
		[Theory(Skip = "requires db access")]
		[InlineData("localhost", 3_306, "root", "thaonad2aeF4Di4zais9IefoodaiBee7", "test", "table1")]
		public async Task RepositoryBaseTests_EndToEnd(string server, int port, string userId, string password, string database, string tableName)
		{
			// Arrange, Act
			var sut = new TestRepository(server, port, userId, password, database);

			// Act
			var now = DateTime.UtcNow;
			var dateTime = await sut.GetDateTimeAsync();

			// Assert
			Assert.InRange(dateTime, now.AddMinutes(-1), now.AddMinutes(1));
			Assert.Equal(ConnectionState.Closed, sut.ConnectionState);

			// Act
			await sut.SafeCreateTableAsync(
				tableName,
				$@"CREATE TABLE `{database}`.`{tableName}` (
					`id` SMALLINT(2) UNSIGNED NOT NULL,
					`name` VARCHAR(100) NOT NULL,
					PRIMARY KEY (`id`)
				);");

			// Assert
			Assert.True(await sut.CheckTableExistsAsync(tableName));

			// Act
			await sut.ExecuteAsync($"DELETE FROM `{database}`.`{tableName}` WHERE id>=0;");

			await sut.ExecuteAsync($"INSERT `{database}`.`{tableName}`(id, name) VALUES (@id, @name);", new[]
			{
				new { id = 1, name = "test", },
			});

			var results = (await sut.QueryAsync<(short, string)>($"SELECT * FROM `{database}`.`{tableName}`;").WaitAsync()).ToList();

			// Assert
			Assert.NotEmpty(results);
			Assert.Single(results);
			Assert.Equal(1, results[0].Item1);
			Assert.Equal("test", results[0].Item2);

			// Act
			await sut.SafeDropTableAsync(tableName);

			// Assert
			Assert.False(await sut.CheckTableExistsAsync(tableName));

			await sut.SafeDropDatabaseAsync(database);

			// Arrange
			sut.Dispose();
		}

		[Theory(Skip = "requires db access")]
		[InlineData("localhost", 3_306, "root", "thaonad2aeF4Di4zais9IefoodaiBee7", "test", "table2")]
		public async Task RepositoryBaseTests_TransactionTest(string server, int port, string userId, string password, string database, string tableName)
		{
			using var sut = new TestRepository(server, port, userId, password, database);

			await sut.SafeDropTableAsync(tableName);

			Assert.False(await sut.CheckTableExistsAsync(tableName));

			// make a table
			await sut.SafeCreateTableAsync(
				tableName,
				$@"CREATE TABLE `test`.`{tableName}` (
					`id` SMALLINT NOT NULL,
					`name` VARCHAR(100) NOT NULL,
					PRIMARY KEY (`id`)
				);");

			IEnumerable<(short id, string name)> results;

			using var transaction = sut.BeginTransaction();

			// check it's empty
			results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`").WaitAsync();

			Assert.Empty(results);

			// add to it
			await sut.ExecuteAsync(
				$@"insert `test`.`{tableName}` (id, name) values (@id, @name);",
				new { id = 1, name = "test", },
				transaction);

			// check it's not empty
			results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`").WaitAsync();

			Assert.NotEmpty(results);

			// rollback the transaction
			transaction.Rollback();

			// check the table is empty
			results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`").WaitAsync();

			Assert.Empty(results);

			await sut.SafeDropTableAsync(tableName);

			// drop the table
			Assert.False(await sut.CheckTableExistsAsync(tableName));

			await sut.SafeDropDatabaseAsync("test");
		}

		[Theory(Skip = "requires db access")]
		[InlineData("localhost", 3_306, "root", "thaonad2aeF4Di4zais9IefoodaiBee7")]
		public async Task RepositoryBaseTests_OpenAnOpenConnection(string server, int port, string userId, string password, string? database = default)
		{
			using var sut = new TestRepository(server, port, userId, password, database);

			using (var transaction = sut.BeginTransaction())
			{
				await sut.GetDateTimeAsync();
			}

			using (var transaction = sut.BeginTransaction())
			{
				await sut.GetDateTimeAsync();
			}
		}

		[Theory]
		[InlineData("localhost", 3_306, "movietimes", "xiebeiyoothohYaidieroh8ahchohphi")]
		public async Task RepositoryBaseTests_ReturnDateTime(string server, int port, string userId, string password, string? database = default)
		{
			using var sut = new TestRepository(server, port, userId, password, database);

			var results = sut.QueryAsync<DateTimeResult>("select now() now union all select now();");

			var now = DateTime.UtcNow;
			var count = 0;

			await foreach (var result in results)
			{
				count++;
				Assert.NotNull(result);
				Assert.NotEqual(default, result.Now);
				Assert.Equal(DateTimeKind.Unspecified, result.Now!.Value.Kind);
				Assert.InRange(result.Now!.Value, now.AddSeconds(-3), now.AddSeconds(3));
			}

			Assert.Equal(2, count);
		}

		private class DateTimeResult
		{
			public DateTime? Now { get; set; }
		}

		[Theory]
		[InlineData("localhost", 3_306, "movietimes", "xiebeiyoothohYaidieroh8ahchohphi")]
		public async Task RepositoryBaseTests_Queries(string server, int port, string userId, string password, string? database = default)
		{
			using var sut = new TestRepository(server, port, userId, password, database);

			var sql = @"select
					c.id cinemaId,
					c.name cinemaName,
					f.title filmTitle,
					f.duration filmDuration,
					s.time showDateTime
				from `cineworld`.`show` s
					join `cineworld`.`cinema` c on s.cinemaId = c.id
					join `cineworld`.`film` f on s.filmEdi = f.edi where c.id in (23) order by c.id, s.time, f.title;";

			var results = sut.QueryAsync<QueryResult>(sql);

			var now = DateTime.UtcNow;
			var count = 0;

			await foreach (var result in results)
			{
				count++;
				Assert.NotNull(result);
				Assert.InRange(result.CinemaId ?? 0, (short)1, short.MaxValue);
				Assert.NotNull(result.CinemaName);
				Assert.NotEmpty(result.CinemaName);
				Assert.NotNull(result.FilmTitle);
				Assert.NotEmpty(result.FilmTitle);
				Assert.NotEqual(default, result.ShowDateTime);
				Assert.Equal(DateTimeKind.Unspecified, result.ShowDateTime!.Value.Kind);
				Assert.InRange(result.ShowDateTime!.Value, now.AddYears(-1), now.AddYears(1));
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		private class QueryResult
		{
			public short? CinemaId { get; set; }
			public string? CinemaName { get; set; }
			public string? FilmTitle { get; set; }
			public short? FilmDuration { get; set; }
			public DateTime? ShowDateTime { get; set; }
		}
	}

	public static class ExtensionMethods
	{
		public async static Task<IEnumerable<T>> WaitAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
		{
			var results = new List<T>();

			var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator();

			while (await asyncEnumerator.MoveNextAsync())
			{
				results.Add(asyncEnumerator.Current);
			}

			await asyncEnumerator.DisposeAsync();

			return results;
		}
	}
}
