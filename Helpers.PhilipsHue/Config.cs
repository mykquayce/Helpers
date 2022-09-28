using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace Helpers.PhilipsHue;

public record Config(string? PhysicalAddress, string? HostName, string Username)
	: IOptions<Config>
{
	public static readonly string? DefaultPhysicalAddress = null;
	public const string DefaultHostName = "localhost";
	public const string DefaultUsername = "username";

	public Config() : this(DefaultPhysicalAddress, DefaultHostName, DefaultUsername) { }

	public Uri? BaseAddress => HostName is not null ? new UriBuilder("http", HostName).Uri : null;

	public Config Value => this;

	public static Config Defaults => new();
}
