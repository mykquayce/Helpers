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

	[Theory]
	[InlineData("localhost", 3_306, "new_schema", "table1", true)]
	public async Task RepositoryBaseTests_EndToEnd(string server, ushort port, string database, string tableName, bool secure)
	{
		// Arrange, Act
		var config = new Config(server, port, default, _username, _password, secure);
		using var connection = config.DbConnection;

		var sut = new TestRepository(connection);

		// Act
		var now = DateTime.UtcNow;
		var dateTime = await sut.GetDateTimeAsync();

		// Assert
		Assert.InRange(dateTime, now.AddMinutes(-1), now.AddMinutes(1));

		// Act
		try
		{
			await sut.ExecuteAsync($"CREATE DATABASE `{database}`;");

			await sut.ExecuteAsync(
				$@"CREATE TABLE `{database}`.`{tableName}` (
					`id` SMALLINT(2) UNSIGNED NOT NULL,
					`name` VARCHAR(100) NOT NULL,
					PRIMARY KEY (`id`)
				);");

			// Assert
			Assert.Equal(1,
				await sut.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM `INFORMATION_SCHEMA`.`TABLES` WHERE `TABLE_NAME` = @tableName LIMIT 1;", new { tableName, }));

			// Act
			var @params = new { id = 1, name = "test", };
			await sut.ExecuteAsync($"INSERT `{database}`.`{tableName}` (id, name) VALUES (@id, @name);", @params);

			var results = await sut.QueryAsync<(short, string)>($"SELECT * FROM `{database}`.`{tableName}`;")
				.ToListAsync();

			// Assert
			Assert.NotEmpty(results);
			Assert.Single(results);
			Assert.Equal(1, results[0].Item1);
			Assert.Equal("test", results[0].Item2);
		}
		finally
		{
			await sut.ExecuteAsync($"DROP TABLE `{database}`.`{tableName}`;");
			await sut.ExecuteAsync($"DROP DATABASE `{database}`;");
		}
	}

	[Theory]
	[InlineData(3, "localhost", 3_306)]
	public async Task RepositoryBaseTests_OpenAnOpenConnection(int count, string server, ushort port)
	{
		var config = new Config(server, port, default, _username, _password);
		using var connection = config.DbConnection;

		var sut = new TestRepository(connection);

		while (count-- > 0)
		{
			using var transaction = sut.BeginTransaction();
			await sut.GetDateTimeAsync();
		}
	}

	[Theory]
	[InlineData("localhost", 3_306)]
	public async Task RepositoryBaseTests_ReturnDateTime(string server, ushort port)
	{
		var config = new Config(server, port, default, _username, _password);
		using var connection = config.DbConnection;

		var sut = new TestRepository(connection);

		var results = await sut.QueryAsync<DateTime>("select now() union all select now();")
			.ToListAsync();

		var now = DateTime.UtcNow;

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(2, results.Count);
		Assert.DoesNotContain(default, results);

		foreach (var result in results)
		{
			Assert.NotEqual(default, result);
			Assert.Equal(DateTimeKind.Unspecified, result.Kind);
			Assert.InRange(result, now.AddSeconds(-3), now.AddSeconds(3));
		}
	}
}
