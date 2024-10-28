using Dapper;
using System.Data;
using System.Net.Sockets;

namespace Helpers.MySql;

public abstract class RepositoryBase(IDbConnection connection)
{
	private void Connect()
	{
		if ((connection.State & ConnectionState.Open) != 0)
		{
			return;
		}

		var exceptions = new List<Exception>();
		{
			using var cts = new CancellationTokenSource(millisecondsDelay: 30_000);

			while (connection.State != ConnectionState.Open
				&& !cts.IsCancellationRequested)
			{
				try
				{
					connection.Open();
				}
				catch (AggregateException ex)
					when (ex.Message == "One or more errors occurred. (Connection refused)"
						&& ex.InnerExceptions.FirstOrDefault() is SocketException)
				{
					exceptions.Add(ex);
					Thread.Sleep(millisecondsTimeout: 2_000);
				}
			}
		}

		if ((connection.State & ConnectionState.Open) == 0)
		{
			var message = exceptions.Last().Message;
			throw new AggregateException(message, exceptions);
		}
	}

	protected Task<int> ExecuteAsync(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(sql);

		Connect();

		return connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType)
			.SafeAwaitAsync();
	}

	protected Task<T?> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(sql);

		Connect();

		return connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType)
			.SafeAwaitAsync();
	}

	protected async IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(sql);

		Connect();

		var results = await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType)
			.SafeAwaitAsync();

		foreach (var result in results)
		{
			yield return result;
		}
	}

	#region Transactions
	protected IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(true, Enum.IsDefined(isolationLevel));

		Connect();

		return connection.BeginTransaction(isolationLevel);
	}
	#endregion Transactions
}
