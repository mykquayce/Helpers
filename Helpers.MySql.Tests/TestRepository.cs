using System.Data;

namespace Helpers.MySql.Tests;

public class TestRepository : RepositoryBase, ITestRepository
{
	public TestRepository(IDbConnection connection)
		: base(connection)
	{ }

	public IDbTransaction BeginTransaction() => base.BeginTransaction();
	public Task<int> ExecuteAsync(string sql, object? param = default, IDbTransaction? transaction = default) => base.ExecuteAsync(sql, param, transaction);
	public Task<T> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default) => base.ExecuteScalarAsync<T>(sql, param, transaction);
	public Task<DateTime> GetDateTimeAsync() => base.ExecuteScalarAsync<DateTime>("SELECT NOW();");
	public IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default) => base.QueryAsync<T>(sql, param, transaction);
}
