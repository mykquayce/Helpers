using Dapper;
using Dawn;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Helpers.MySql
{
	public abstract class RepositoryBase : IDisposable
	{
		private IDbConnection _connection;
		private readonly ILogger _logger;

		protected RepositoryBase(
			string connectionString,
			ILogger<RepositoryBase> logger = default)
		{
			if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

			_connection = new MySqlConnection(connectionString);
			_logger = logger;
		}

		public void Dispose()
		{
			_connection?.Close();
			_connection?.Dispose();
		}

		protected ConnectionState ConnectionState => _connection?.State ?? ConnectionState.Closed;

		protected void Connect()
		{
			_connection?.Open();
		}

		protected Task<int> ExecuteAsync(string sql, object param = default, IDbTransaction transaction = default, int? commandTimeout = default, CommandType? commandType = default)
		{
			Guard.Argument(() => sql).NotNull().NotEmpty().NotWhiteSpace();

			return SafeExecuteAsync(() => _connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType));
		}

		protected Task<T> ExecuteScalarAsync<T>(string sql, object param = default, IDbTransaction transaction = default, int? commandTimeout = default, CommandType? commandType = default)
		{
			Guard.Argument(() => sql).NotNull().NotEmpty().NotWhiteSpace();

			return SafeExecuteAsync(() => _connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, commandType));
		}

		protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = default, IDbTransaction transaction = default, int? commandTimeout = default, CommandType? commandType = default)
		{
			Guard.Argument(() => sql).NotNull().NotEmpty().NotWhiteSpace();

			return SafeExecuteAsync(() => _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType));
		}

		protected IDbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			Guard.Argument(() => isolationLevel).Defined();

			return _connection.BeginTransaction(isolationLevel);
		}

		protected async Task<bool> CheckTableExistsAsync(string tableName)
		{
			Guard.Argument(() => tableName).NotNull().NotEmpty().NotWhiteSpace().Matches("^[$0-9A-Z_a-z]{1,64}$");

			return await ExecuteScalarAsync<string>($"SHOW TABLES FROM {_connection.Database} LIKE @tableName;", new { tableName, }) == tableName;
		}

		protected async Task SafeCreateTableAsync(string tableName, string sql)
		{
			Guard.Argument(() => tableName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => sql).NotNull().NotEmpty().NotWhiteSpace();

			if (await CheckTableExistsAsync(tableName))
			{
				SafeDropTableAsync(tableName);
			}

			ExecuteAsync(sql);
		}

		protected async Task SafeDropTableAsync(string tableName)
		{
			Guard.Argument(() => tableName).NotNull().NotEmpty().NotWhiteSpace().Matches("^[$0-9A-Z_a-z]{1,64}$");

			if (await CheckTableExistsAsync(tableName))
			{
				ExecuteAsync($"DROP TABLE {_connection.Database}.{tableName};");
			}
		}

		private static readonly IDictionary<ExceptionTypes, int> _exceptions = new SafeDictionary<ExceptionTypes, int>();

		[Flags]
		private enum ExceptionTypes : byte
		{
			None = 0,
			TargetMachineActivelyRefused = 1,
		}

		private async Task<T> SafeExecuteAsync<T>(Func<Task<T>> func)
		{
			Guard.Argument(() => func).NotNull();

			try
			{
				var result = await func();
				_exceptions.Clear();
				return result;
			}
			catch (MySqlException ex) when (
				ex.Message == "Unable to connect to any of the specified MySQL hosts."
				&& (ex.InnerException?.Message.StartsWith("No connection could be made because the target machine actively refused it.") ?? false))
			{
				// add this exception to the pile
				_exceptions[ExceptionTypes.TargetMachineActivelyRefused]++;

				ex.Data.Add("attempt", _exceptions[ExceptionTypes.TargetMachineActivelyRefused]);

				_logger?.LogCritical(ex, ex.Message);

				// if it's happenned ten consequetive times, 'splode
				if (_exceptions[ExceptionTypes.TargetMachineActivelyRefused] >= 10)
				{
					throw;
				}

				// pause...
				await Task.Delay(millisecondsDelay: 3_000);

				// ...then try again
				return await SafeExecuteAsync(func);
			}
			catch (Exception ex)
			{
				foreach (var (key, value) in _exceptions)
				{
					ex.Data.Add(key, value);
				}

				_logger?.LogCritical(ex, ex.Message);

				throw;
			}
		}
	}

	public class SafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly IDictionary<TKey, TValue> _dictionary;

		public SafeDictionary() => _dictionary = new Dictionary<TKey, TValue>();
		public SafeDictionary(IDictionary<TKey, TValue> dictionary) => _dictionary = new Dictionary<TKey, TValue>(dictionary);
		public SafeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) => _dictionary = new Dictionary<TKey, TValue>(collection);
		public SafeDictionary(IEqualityComparer<TKey> comparer) => _dictionary = new Dictionary<TKey, TValue>(comparer);
		public SafeDictionary(int capacity) => _dictionary = new Dictionary<TKey, TValue>(capacity);
		public SafeDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) => _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
		public SafeDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) => _dictionary = new Dictionary<TKey, TValue>(collection, comparer);
		public SafeDictionary(int capacity, IEqualityComparer<TKey> comparer) => _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);

		public TValue this[TKey key]
		{
			get
			{
				if (_dictionary.ContainsKey(key))
				{
					return _dictionary[key];
				}

				_dictionary.Add(key, default);

				return default;
			}

			set
			{
				if (_dictionary.ContainsKey(key))
				{
					_dictionary[key] = value;
				}
				else
				{
					_dictionary.Add(key, value);
				}
			}
		}

		public ICollection<TKey> Keys => _dictionary.Keys;
		public ICollection<TValue> Values => _dictionary.Values;
		public int Count => _dictionary.Count;
		public bool IsReadOnly => _dictionary.IsReadOnly;
		public void Add(TKey key, TValue value) => _dictionary.Add(key, value);
		public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item);
		public void Clear() => _dictionary.Clear();
		public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
		public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
		public bool Remove(TKey key) => _dictionary.Remove(key);
		public bool Remove(KeyValuePair<TKey, TValue> item) => _dictionary.Remove(item);
		public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
		IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
	}
}
