using Helpers.Json.Converters;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;

namespace Helpers.Elgato.Tests.Fixtures
{
	public class ConfigFixture
	{
		public ConfigFixture()
		{
			var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			Addresses = @base.Configuration.GetSection("Elgato").GetSection("EndPoints").GetType<Addresses>();
			EndPoint = new(Addresses.IPAddress, 9_123);
			SSHConfig = @base.Configuration.GetSection("SSH").GetType<Helpers.SSH.Services.Concrete.SSHService.Config>();
		}

		public Addresses Addresses { get; }
		public IPEndPoint EndPoint { get; }
		public Helpers.SSH.Services.Concrete.SSHService.Config SSHConfig { get; }
	}

	public record Addresses(
		[property: JsonConverter(typeof(JsonIPAddressConverter))] IPAddress IPAddress,
		[property: JsonConverter(typeof(JsonPhysicalAddressConverter))] PhysicalAddress PhysicalAddress);
}
