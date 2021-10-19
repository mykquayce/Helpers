using Dapper;
using Dawn;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace Helpers.MySql;

public abstract class RepositoryBase : IDisposable
{
	private readonly IDbConnection _connection;
	private const string _namePattern = @"^[$0-9A-Z_a-z]{1,64}$";

	protected RepositoryBase(IOptions<Config> options)
	{
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;
		_connection = new MySqlConnection(config.ConnectionString);
	}

	public void Dispose()
	{
		Dispose(disposing: true);

		GC.SuppressFinalize(obj: this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			_connection?.Close();
			_connection?.Dispose();
		}
	}

	protected ConnectionState ConnectionState => _connection?.State ?? ConnectionState.Closed;

	protected void Connect()
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

		return SafeExecuteAsync(() => _connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType));
	}

	protected Task<T> ExecuteScalarAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		Guard.Argument(sql).NotNull().NotEmpty().NotWhiteSpace();

		return SafeExecuteAsync(() => _connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType));
	}

	protected async IAsyncEnumerable<T> QueryAsync<T>(string sql, object? param = default, IDbTransaction? transaction = default, int? commandTimeout = default, CommandType? commandType = default)
	{
		Guard.Argument(sql).NotNull().NotEmpty().NotWhiteSpace();

		var results = await SafeExecuteAsync(() => _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));

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

	#region Tables
	protected async Task<bool> CheckTableExistsAsync(string tableName, IDbTransaction? transaction = default)
	{
		Guard.Argument(tableName).NotNull().NotEmpty().NotWhiteSpace().Matches(_namePattern);

		var result = await ExecuteScalarAsync<string>(
			sql: $"SHOW TABLES FROM {_connection.Database} LIKE @tableName;",
			param: new { tableName, },
			transaction: transaction);

		return result == tableName;
	}

	protected async Task SafeCreateTableAsync(string tableName, string sql, IDbTransaction? transaction = default)
	{
		Guard.Argument(tableName).NotNull().NotEmpty().NotWhiteSpace().Matches(_namePattern);
		Guard.Argument(sql).NotNull().NotEmpty().NotWhiteSpace();

		if (await CheckTableExistsAsync(tableName, transaction: transaction))
		{
#pragma warning disable CS4014
			SafeDropTableAsync(tableName, transaction: transaction);
#pragma warning restore CS4014
		}

#pragma warning disable CS4014
		ExecuteAsync(sql, transaction: transaction);
#pragma warning restore CS4014
	}

	protected async Task SafeDropTableAsync(string tableName, IDbTransaction? transaction = default)
	{
		Guard.Argument(tableName).NotNull().NotEmpty().NotWhiteSpace().Matches(_namePattern);

		if (await CheckTableExistsAsync(tableName, transaction))
		{
#pragma warning disable CS4014
			ExecuteAsync($"DROP TABLE {_connection.Database}.{tableName};", transaction: transaction);
#pragma warning restore CS4014
		}
	}
	#endregion Tables

	#region Databases
	protected Task<int> SafeCreateDatabaseAsync(string? databaseName = default, IDbTransaction? transaction = default)
	{
		// break down the connection string
		var dictionary = _connection.ConnectionString.ToDictionary();

		// get the database name
		if (databaseName == default
			&& dictionary.ContainsKey("database"))
		{
			databaseName = dictionary["database"];
		}

		// check it
		Guard.Argument(databaseName!).NotNull().NotEmpty().NotWhiteSpace().Matches(_namePattern);

		// build a new connection string one without the database
		var temp = string.Join(';', from kvp in dictionary
									where !string.Equals(kvp.Key, "database", StringComparison.OrdinalIgnoreCase)
									select string.Join("=", kvp.Key, kvp.Value));

		// create database using the new connection string
		using var connection = new MySqlConnection(temp);

		return connection.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{databaseName}`;", transaction: transaction);
	}

	protected Task<int> SafeDropDatabaseAsync(string databaseName, IDbTransaction? transaction = default)
	{
		Guard.Argument(databaseName).NotNull().NotEmpty().NotWhiteSpace().Matches(_namePattern);

		return ExecuteAsync($"DROP DATABASE IF EXISTS `{databaseName}`;", transaction: transaction);
	}
	#endregion Databases

	private static readonly IDictionary<ExceptionTypes, int> _exceptions = new Collections.ExceptionDictionary();

	private async Task<T> SafeExecuteAsync<T>(Func<Task<T>> func)
	{
		Guard.Argument(func).NotNull();

		try
		{
			var result = await func();
			_exceptions.Clear();
			return result;
		}
		catch (Exception ex)
		{
			await ProcessExceptionAsync(ex);
			return await SafeExecuteAsync(func);
		}
	}

	private async Task ProcessExceptionAsync(Exception exception)
	{
		Guard.Argument(exception).NotNull();

		switch (exception)
		{
			// cannot connect to the database server
			case MySqlException mySqlException when
				mySqlException.Message.Equals("Unable to connect to any of the specified MySQL hosts.", StringComparison.OrdinalIgnoreCase)
				&& mySqlException.InnerException is AggregateException aggregateException
				&& aggregateException.InnerExceptions.Count == 1
				&& aggregateException.InnerExceptions[0].Message.StartsWith("No connection could be made because the target machine actively refused it.", StringComparison.OrdinalIgnoreCase):
				{
					_exceptions[ExceptionTypes.TargetMachineActivelyRefused]++;

					exception.Data.Add("attempt", _exceptions[ExceptionTypes.TargetMachineActivelyRefused]);

					// if we've tried more than ten times, 'splode
					if (_exceptions[ExceptionTypes.TargetMachineActivelyRefused] > 10)
					{
						throw exception;
					}

					// pause...
					await Task.Delay(millisecondsDelay: 3_000);

					break;
				}
			// the schema doesn't exist
			case MySqlException _ when
				Regex.IsMatch(exception.Message, "^Unknown database '[$0-9A-Z_a-z]{1,64}'$"):
			case MySqlException mySqlException when
				Regex.IsMatch(mySqlException.Message, "Authentication to host '[$0-9A-Z_a-z]{1,64}' for user '[$0-9A-Z_a-z]{1,64}' using method 'mysql_native_password' failed with message: Unknown database '[$0-9A-Z_a-z]{1,64}'")
				&& mySqlException.InnerException is MySqlException mySqlInnerException
				&& Regex.IsMatch(mySqlInnerException.Message, "^Unknown database '[$0-9A-Z_a-z]{1,64}'$"):
				{
					_exceptions[ExceptionTypes.UnknownDatabase]++;

					// if we've tried before, 'splode
					if (_exceptions[ExceptionTypes.UnknownDatabase] > 1)
					{
						throw exception;
					}

					await SafeCreateDatabaseAsync();

					break;
				}
			default:
				{
					foreach (var (key, value) in _exceptions)
					{
						exception.Data.Add(key, value);
					}

					throw exception;
				}
		}
	}
}
