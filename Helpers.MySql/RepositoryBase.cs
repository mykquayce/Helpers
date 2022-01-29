using Dapper;
using Dawn;
using System.Data;

namespace Helpers.MySql;

public abstract class RepositoryBase
{
	private readonly IDbConnection _connection;

	protected RepositoryBase(IDbConnection connection)
	{
		_connection = Guard.Argument(connection).NotNull().Value;
	}

	private void Connect()
	{
		if ((_connection.State & ConnectionState.Open) != 0)
		{
			return;
		}

		_connection.Open();
	}

	protected Task<int> ExecuteAsync(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		Guard.Argument(sql).NotNull().NotEmpty().NotWhiteSpace();

		Connect();

		return _connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType)
			.SafeAwaitAsync();
	}

	protected Task<T> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		Guard.Argument(sql).NotNull().NotEmpty().NotWhiteSpace();

		Connect();

		return _connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType)
			.SafeAwaitAsync();
	}

	protected async IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		Guard.Argument(sql).NotNull().NotEmpty().NotWhiteSpace();

		Connect();

		var results = await _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType)
			.SafeAwaitAsync();

		foreach (var result in results)
		{
			yield return result;
		}
	}

	#region Transactions
	protected IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
	{
		Guard.Argument(isolationLevel).Defined();

		Connect();

		return _connection.BeginTransaction(isolationLevel);
	}
	#endregion Transactions
}
