using Microsoft.Extensions.Options;

namespace Helpers.NetworkDiscovery;

public record Config(Uri BaseAddress)
	: IOptions<Config>
{
	public static readonly Uri DefaultBaseAddress = new("https://networkdiscovery/");
	public Config()
		: this(DefaultBaseAddress)
	{ }

	public Config Value => this;

	public static Config Defaults => new();
}
