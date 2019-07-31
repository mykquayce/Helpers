using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Helpers.MySql.Tests
{
	public class TestRepository : RepositoryBase
	{
		public TestRepository(string server, int port, string userId, string password, string? database = default)
			: base(server, port, userId, password, database)
		{ }

		public new ConnectionState ConnectionState => base.ConnectionState;

		public IDbTransaction BeginTransaction() => base.BeginTransaction();
		public new void Connect() => base.Connect();
		public new Task<bool> CheckTableExistsAsync(string tableName, IDbTransaction? transaction = default) => base.CheckTableExistsAsync(tableName, transaction);
		public Task<int> ExecuteAsync(string sql, object? param = default, IDbTransaction? transaction = default) => base.ExecuteAsync(sql, param, transaction);
		public Task<T> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default) => base.ExecuteScalarAsync<T>(sql, param, transaction);
		public Task<DateTime> GetDateTimeAsync() => base.ExecuteScalarAsync<DateTime>("SELECT NOW();");
		public IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default) => base.QueryAsync<T>(sql, param, transaction);
		public new Task SafeCreateDatabaseAsync(string? databaseName = default, IDbTransaction? transaction = default) => base.SafeCreateDatabaseAsync(databaseName, transaction);
		public new Task SafeCreateTableAsync(string tableName, string sql, IDbTransaction? transaction = default) => base.SafeCreateTableAsync(tableName, sql, transaction);
		public new Task<int> SafeDropDatabaseAsync(string databaseName, IDbTransaction? transaction = default) => base.SafeDropDatabaseAsync(databaseName, transaction);
		public new Task SafeDropTableAsync(string tableName, IDbTransaction? transaction = default) => base.SafeDropTableAsync(tableName, transaction);
	}
}
