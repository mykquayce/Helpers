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
		[InlineData(
			"server=localhost;port=3306;user id=root;password=xiebeiyoothohYaidieroh8ahchohphi;database=test;",
			"test",
			"table1")]
		public async Task RepositoryBaseTests_EndToEnd(string connectionString, string databaseName, string tableName)
		{
			// Arrange, Act
			var sut = new TestRepository(connectionString);

			// Act
			var now = DateTime.UtcNow;
			var dateTime = await sut.GetDateTimeAsync();

			// Assert
			Assert.InRange(dateTime, now.AddMinutes(-1), now.AddMinutes(1));
			Assert.Equal(ConnectionState.Closed, sut.ConnectionState);

			// Act
			await sut.SafeCreateTableAsync(
				tableName,
				$@"CREATE TABLE `{databaseName}`.`{tableName}` (
					`id` SMALLINT(2) UNSIGNED NOT NULL,
					`name` VARCHAR(100) NOT NULL,
					PRIMARY KEY (`id`)
				);");

			// Assert
			Assert.True(await sut.CheckTableExistsAsync(tableName));

			// Act
			await sut.ExecuteAsync($"DELETE FROM `{databaseName}`.`{tableName}` WHERE id>=0;");

			await sut.ExecuteAsync($"INSERT `{databaseName}`.`{tableName}`(id, name) VALUES (@id, @name);", new[]
			{
				new { id = 1, name = "test", },
			});

			var results = (await sut.QueryAsync<(short, string)>($"SELECT * FROM `{databaseName}`.`{tableName}`;")).ToList();

			// Assert
			Assert.NotEmpty(results);
			Assert.Single(results);
			Assert.Equal(1, results[0].Item1);
			Assert.Equal("test", results[0].Item2);

			// Act
			await sut.SafeDropTableAsync(tableName);

			// Assert
			Assert.False(await sut.CheckTableExistsAsync(tableName));

			await sut.SafeDropDatabaseAsync(databaseName);

			// Arrange
			sut.Dispose();
		}

		[Theory(Skip = "requires db access")]
		[InlineData("server=localhost;port=3306;user id=root;password=xiebeiyoothohYaidieroh8ahchohphi;database=test;", "table2")]
		public async Task RepositoryBaseTests_TransactionTest(string connectionString, string tableName)
		{
			using var sut = new TestRepository(connectionString);

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
			results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`");

			Assert.Empty(results);

			// add to it
			await sut.ExecuteAsync(
				$@"insert `test`.`{tableName}` (id, name) values (@id, @name);",
				new { id = 1, name = "test", },
				transaction);

			// check it's not empty
			results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`");

			Assert.NotEmpty(results);

			// rollback the transaction
			transaction.Rollback();

			// check the table is empty
			results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`");

			Assert.Empty(results);

			await sut.SafeDropTableAsync(tableName);

			// drop the table
			Assert.False(await sut.CheckTableExistsAsync(tableName));

			await sut.SafeDropDatabaseAsync("test");
		}

		[Theory(Skip = "requires db access")]
		[InlineData("server=localhost;port=3306;user id=root;password=xiebeiyoothohYaidieroh8ahchohphi;")]
		public async Task RepositoryBaseTests_OpenAnOpenConnection(string connectionString)
		{
			using var sut = new TestRepository(connectionString);

			using (var transaction = sut.BeginTransaction())
			{
				await sut.GetDateTimeAsync();
			}

			using (var transaction = sut.BeginTransaction())
			{
				await sut.GetDateTimeAsync();
			}
		}
	}
}