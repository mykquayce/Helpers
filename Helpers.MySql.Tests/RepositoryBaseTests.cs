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
		[Theory]
		[InlineData("server=localhost;port=4040;user id=root;password=xiebeiyoothohYaidieroh8ahchohphi;database=test;")]
		public async Task RepositoryBaseTests_(string connectionString)
		{
			// Arrange, Act
			var sut = new Repository(connectionString);

			// Act
			var now = DateTime.UtcNow;
			var dateTime = await sut.GetDateTimeAsync();

			// Assert
			Assert.InRange(dateTime, now.AddMinutes(-1), now.AddMinutes(1));
			Assert.Equal(ConnectionState.Closed, sut.ConnectionState);

			// Act
			await sut.SafeCreateTableAsync(
				"table1",
				@"CREATE TABLE `test`.`table1` (
					`id` SMALLINT(2) UNSIGNED NOT NULL,
					`name` VARCHAR(100) NOT NULL,
					PRIMARY KEY (`id`)
				);");

			// Assert
			Assert.True(await sut.CheckTableExistsAsync("table1"));
			Assert.False(await sut.CheckTableExistsAsync("table2"));

			// Act
			await sut.ExecuteAsync("DELETE FROM `test`.`table1` WHERE id>=0;");

			await sut.ExecuteAsync("INSERT `test`.`table1`(id, name) VALUES (@id, @name);", new[]
			{
				new { id = 1, name = "test", },
			});

			var results = (await sut.QueryAsync<(short, string)>("SELECT * FROM `test`.`table1`;"))?.ToList();

			// Assert
			Assert.NotNull(results);
			Assert.NotEmpty(results);
			Assert.Single(results);
			Assert.Equal(1, results[0].Item1);
			Assert.Equal("test", results[0].Item2);

			// Act
			await sut.SafeDropTableAsync("table1");

			// Assert
			Assert.False(await sut.CheckTableExistsAsync("table1"));

			// Arrange
			sut.Dispose();
		}

		private class Repository : RepositoryBase
		{
			public Repository(string connectionString)
				: base(connectionString)
			{ }

			public new ConnectionState ConnectionState => base.ConnectionState;

			public new void Connect() => base.Connect();

			public new Task<bool> CheckTableExistsAsync(string tableName) => base.CheckTableExistsAsync(tableName);

			public new Task SafeCreateTableAsync(string tableName, string sql) => base.SafeCreateTableAsync(tableName, sql);
			public new Task SafeDropTableAsync(string tableName) => base.SafeDropTableAsync(tableName);

			public Task<DateTime> GetDateTimeAsync() => base.ExecuteScalarAsync<DateTime>("SELECT NOW();");

			public Task<int> ExecuteAsync(string sql, object param = default) => base.ExecuteAsync(sql, param);

			public Task<T> ExecuteScalarAsync<T>(string sql, object param = default) => base.ExecuteScalarAsync<T>(sql, param);

			public Task<IEnumerable<T>> QueryAsync<T>(string sql) => base.QueryAsync<T>(sql);
		}
	}
}
