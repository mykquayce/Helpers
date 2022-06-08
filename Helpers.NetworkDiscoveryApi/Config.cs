using Microsoft.Extensions.Options;

namespace Helpers.NetworkDiscoveryApi;

public record Config(EndPoints EndPoints, Identity.Config Identity, Aliases Aliases)
	: IOptions<Config>
{
	public Config()
		: this(EndPoints.Defaults, Helpers.Identity.Config.Defaults, Aliases.Defaults)
	{ }

	public static Config Defaults => new();

	public Config Value => this;
}
