using System.Data;
using Xunit;

namespace Helpers.MySql.Tests;

public class RepositoryBaseTests : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
{
	private readonly string _username, _password;

	public RepositoryBaseTests(Helpers.XUnitClassFixtures.UserSecretsFixture fixture)
	{
		_username = fixture["MySql:Username"];
		_password = fixture["MySql:Password"];
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "<Pending>")]
	[Theory(Skip = "requires db access")]
	[InlineData("localhost", 3_306, "test", "table1")]
	public async Task RepositoryBaseTests_EndToEnd(string server, uint port, string database, string tableName)
	{
		// Arrange, Act
		var config = new Config(server, port, database, _username, _password);

		using var sut = new TestRepository(config);

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

		var results = await sut.QueryAsync<(short, string)>($"SELECT * FROM `{database}`.`{tableName}`;").ToListAsync();

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

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "<Pending>")]
	[Theory(Skip = "requires db access")]
	[InlineData("localhost", 3_306, "test", "table2")]
	public async Task RepositoryBaseTests_TransactionTest(string server, uint port, string database, string tableName)
	{
		var config = new Config(server, port, database, _username, _password);
		using var sut = new TestRepository(config);

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
		results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`").ToListAsync();

		Assert.Empty(results);

		// add to it
		await sut.ExecuteAsync(
			$@"insert `test`.`{tableName}` (id, name) values (@id, @name);",
			new { id = 1, name = "test", },
			transaction);

		// check it's not empty
		results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`").ToListAsync();

		Assert.NotEmpty(results);

		// rollback the transaction
		transaction.Rollback();

		// check the table is empty
		results = await sut.QueryAsync<(short id, string name)>($"select * from `test`.`{tableName}`").ToListAsync();

		Assert.Empty(results);

		await sut.SafeDropTableAsync(tableName);

		// drop the table
		Assert.False(await sut.CheckTableExistsAsync(tableName));

		await sut.SafeDropDatabaseAsync("test");
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "<Pending>")]
	[Theory(Skip = "requires db access")]
	[InlineData("localhost", 3_306)]
	public async Task RepositoryBaseTests_OpenAnOpenConnection(string server, uint port, string? database = default)
	{
		var config = new Config(server, port, database!, _username, _password);
		using var sut = new TestRepository(config);

		using (var transaction = sut.BeginTransaction())
		{
			await sut.GetDateTimeAsync();
		}

		using (var transaction = sut.BeginTransaction())
		{
			await sut.GetDateTimeAsync();
		}
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "<Pending>")]
	[Theory(Skip = "requires db access")]
	[InlineData("localhost", 3_306)]
	public async Task RepositoryBaseTests_ReturnDateTime(string server, uint port, string? database = default)
	{
		var config = new Config(server, port, database!, _username, _password);
		using var sut = new TestRepository(config);

		var results = sut.QueryAsync<DateTimeResult>("select now() now union all select now();");

		var now = DateTime.UtcNow;
		var count = 0;

		await foreach (var result in results)
		{
			count++;
			Assert.NotNull(result);
			Assert.NotEqual(default, result.Now);
			Assert.Equal(DateTimeKind.Unspecified, result.Now.Kind);
			Assert.InRange(result.Now, now.AddSeconds(-3), now.AddSeconds(3));
		}

		Assert.Equal(2, count);
	}

	private record DateTimeResult(DateTime Now);
}
