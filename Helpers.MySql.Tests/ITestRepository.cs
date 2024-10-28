using System.Data;

namespace Helpers.MySql.Tests;

public interface ITestRepository
{
	IDbTransaction BeginTransaction();
	Task<int> ExecuteAsync(string sql, object? param = default, IDbTransaction? transaction = default);
	Task<T?> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default);
	Task<DateTime> GetDateTimeAsync(IDbTransaction? transaction = default);
	IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default);
}
