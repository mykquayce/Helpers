using Microsoft.Extensions.Options;

namespace Helpers.SSH;

public record Config(string Host, int Port, string Username, string? Password = default, string? PathToPrivateKey = default)
	: IOptions<Config>
{
	public const string DefaultHost = "localhost";
	public const int DefaultPort = 22;
	public const string DefaultUsername = "root";

	public Config() : this(DefaultHost, DefaultPort, DefaultUsername) { }

	public static Config Default => new();

	#region IOptions implementation
	public Config Value => this;
	#endregion IOptions implementation
}
