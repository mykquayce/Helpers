﻿using System.Data;

namespace Helpers.MySql.Tests;

public class TestRepository(IDbConnection connection)
	: RepositoryBase(connection), ITestRepository
{
	public IDbTransaction BeginTransaction() => base.BeginTransaction();
	public Task<int> ExecuteAsync(string sql, object? param = default, IDbTransaction? transaction = default) => base.ExecuteAsync(sql, param, transaction);
	public Task<T?> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default) => base.ExecuteScalarAsync<T>(sql, param, transaction);
	public Task<DateTime> GetDateTimeAsync(IDbTransaction? transaction = default) => base.ExecuteScalarAsync<DateTime>("SELECT NOW();", transaction: transaction);
	public IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default) => base.QueryAsync<T>(sql, param, transaction);
}
