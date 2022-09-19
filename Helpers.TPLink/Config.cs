using Microsoft.Extensions.Options;

namespace Helpers.TPLink;

public record Config(ushort Port)
	:IOptions<Config>
{
	public const ushort DefaultPort = 9_999;

	public Config() : this(DefaultPort) { }

	public static Config Defaults => new();

	public Config Value => this;
}
