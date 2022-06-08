using Microsoft.Extensions.Options;

namespace Helpers.NetworkDiscoveryApi;

public record EndPoints(Uri IdentityServer, Uri NetworkDiscoveryApi)
	: IOptions<EndPoints>
{
	public static readonly Uri DefaultIdentityServer = new("https://identityserver");
	public static readonly Uri DefaultNetworkDiscoveryApi = new("https://networkdiscovery");

	public EndPoints()
		: this(DefaultIdentityServer, DefaultNetworkDiscoveryApi)
	{ }

	public static EndPoints Defaults => new();

	public EndPoints Value => this;
}
