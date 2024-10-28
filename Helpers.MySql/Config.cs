using Microsoft.Extensions.Options;
using MySqlConnector;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Helpers.MySql;

public record Config(
	string Server,
	uint Port,
	string? Database,
	string UserId,
	string Password,
	bool Secure = false)
	: IOptions<Config>, IParsable<Config>
{
	public const string DefaultServer = "localhost", DefaultUserId = "Root";
	public const string? DefaultDatabase = default;
	public const uint DefaultPort = 3_306;
	public const string DefaultPassword = default;
	public const bool DefaultSecure = false;

	public Config() : this(DefaultServer, DefaultPort, DefaultDatabase, DefaultUserId, DefaultPassword, DefaultSecure) { }

	public static Config Defaults => new();

	#region ioptions implementation
	public Config Value => this;
	#endregion ioptions implementation

	public DbConnectionStringBuilder ConnectionStringBuilder
	{
		get
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(Server);
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Port);
			ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(Port, ushort.MaxValue);
			ArgumentException.ThrowIfNullOrWhiteSpace(Database);
			ArgumentException.ThrowIfNullOrWhiteSpace(UserId);
			ArgumentException.ThrowIfNullOrWhiteSpace(Password);
			var sslMode = Secure ? MySqlSslMode.Required : MySqlSslMode.Preferred;

			return new MySqlConnectionStringBuilder
			{
				Server = this.Server,
				Port = this.Port,
				Database = this.Database,
				UserID = this.UserId,
				Password = this.Password,
				SslMode = sslMode,
			};
		}
	}

	public string ConnectionString => ConnectionStringBuilder.ConnectionString;

	public IDbConnection DbConnection => new MySqlConnection(ConnectionString);

	#region iparsable implementation
	public static Config Parse(string s, IFormatProvider? provider)
	{
		var builder = new MySqlConnectionStringBuilder(s);

		var secure = builder.SslMode switch
		{
			MySqlSslMode.Required => true,
			MySqlSslMode.VerifyCA => true,
			_ => false,
		};

		return new(builder.Server, builder.Port, builder.Database, builder.UserID, builder.Password, secure);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Config result)
	{
		if (string.IsNullOrWhiteSpace(s)) goto fail;
		try
		{
			result = Parse(s, provider);
			return true;
		}
		catch { }
	fail:
		result = default;
		return false;
	}
	#endregion iparsable implementation
}
