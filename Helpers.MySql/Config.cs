using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Helpers.MySql;

public record Config(string Server, uint Port, string Database, string UserId, string Password, MySqlSslMode SslMode = Config.DefaultSslMode)
	: IOptions<Config>
{
	public const MySqlSslMode DefaultSslMode = MySqlSslMode.None;

	#region ioptions implementation
	public Config Value => this;
	#endregion ioptions implementation

	public string ConnectionString
	{
		get
		{
			var builder = new MySqlConnectionStringBuilder
			{
				Server = Server,
				Port = Port,
				Database = Database,
				UserID = UserId,
				Password = Password,
				SslMode = SslMode,
			};

			return builder.ConnectionString;
		}
	}
}
