using Dawn;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;

namespace Helpers.MySql;

public record Config(
	string Server,
	uint Port,
	string? Database,
	string UserId,
	string Password,
	bool Secure = false)
	: IOptions<Config>
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
			var sslMode = Secure ? MySqlSslMode.Required : MySqlSslMode.Preferred;

			return new MySqlConnectionStringBuilder
			{
				Server = Guard.Argument(Server).NotNull().NotEmpty().NotWhiteSpace().Value,
				Port = Guard.Argument(Port).InRange((ushort)1, ushort.MaxValue).Value,
				Database = Guard.Argument(Database).NotEmpty().NotWhiteSpace().Value,
				UserID = Guard.Argument(UserId).NotNull().NotEmpty().NotWhiteSpace().Value,
				Password = Guard.Argument(Password).NotEmpty().NotWhiteSpace().Value,
				SslMode = Guard.Argument(sslMode).Defined().Value,
			};
		}
	}

	public string ConnectionString => ConnectionStringBuilder.ConnectionString;

	public IDbConnection DbConnection => new MySqlConnection(ConnectionString);
}
