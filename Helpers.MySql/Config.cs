using Dawn;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;

namespace Helpers.MySql;

public record Config(
	string Server,
	ushort Port,
	string Database,
	string UserId,
	string? Password,
	string? PasswordFile,
	MySqlSslMode SslMode = Config.DefaultSslMode)
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
			string? password = (Password, PasswordFile) switch
			{
				(null, null) => null,
				(_, null) => Password!,
				(null, _) => GetContents(PasswordFile!),
				(_, _) => throw new ArgumentException("Password and PasswordFile specified.  Please specify just one."),
			};

			var builder = new MySqlConnectionStringBuilder
			{
				Server = Guard.Argument(Server).NotNull().NotEmpty().NotWhiteSpace().Value,
				Port = Guard.Argument(Port).InRange((ushort)1, ushort.MaxValue).Value,
				Database = Guard.Argument(Database).NotNull().NotEmpty().NotWhiteSpace().Value,
				UserID = Guard.Argument(UserId).NotNull().NotEmpty().NotWhiteSpace().Value,
				Password = Guard.Argument(password).NotEmpty().NotWhiteSpace().Value,
				SslMode = Guard.Argument(SslMode).Defined().Value,
			};

			return builder.ConnectionString;
		}
	}

	private static string GetContents(string path)
	{
		Guard.Argument(path).NotNull().NotEmpty().NotWhiteSpace();
		return File.ReadAllText(path);
	}
}
