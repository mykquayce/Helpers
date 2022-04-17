using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.Elgato;

public record Config(
	string Scheme,
	int Port,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonPhysicalAddressConverter))] PhysicalAddress PhysicalAddress)
		: IOptions<Config>
{
	public const string DefaultScheme = "http";
	public const int DefaultPort = 9_123;
	public static readonly PhysicalAddress DefaultPhysicalAddress = PhysicalAddress.Parse("3c6a9d14d765");

	public Config() : this(DefaultScheme, DefaultPort, DefaultPhysicalAddress) { }

	public static Config Defaults => new();

	#region ioptions implementation
	public Config Value => this;
	#endregion ioptions implementation
}
