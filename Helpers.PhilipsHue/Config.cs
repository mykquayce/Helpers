using Microsoft.Extensions.Options;

namespace Helpers.PhilipsHue;

public record Config(string Username, Uri DiscoveryEndPoint)
	: IOptions<Config>
{
	public const string DefaultUsername = "username";
	public static readonly Uri DefaultDiscoveryEndPoint = new("https://discovery.meethue.com/");

	public Config() : this(DefaultUsername, DefaultDiscoveryEndPoint) { }

	public Config Value => this;

	public static Config Defaults => new();
}
