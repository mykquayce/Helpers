using Microsoft.Extensions.Options;

namespace Helpers.GlobalCache;

public record Config(int BufferSize, string HostName, ushort Port)
	: IOptions<Config>
{
	public const int DefaultBufferSize = 1_024;
	public const string DefaultHostName = "localhost";
	public const ushort DefaultPort = 4_998;

	public Config() : this(DefaultBufferSize, DefaultHostName, DefaultPort) { }

	public static Config Defaults => new();

	public Config Value => this;
}
