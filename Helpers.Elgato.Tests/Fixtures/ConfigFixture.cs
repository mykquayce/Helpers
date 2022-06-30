using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.Elgato.Tests.Fixtures;

public class ConfigFixture
{
	public ConfigFixture()
	{
		var userSecretsFixture = new XUnitClassFixtures.UserSecretsFixture();

		Configuration = userSecretsFixture.Configuration;

		Aliases = Helpers.NetworkDiscoveryApi.Aliases.Bind(Configuration.GetSection("aliases"));

		foreach (var (alias, _) in Aliases)
		{
			var ipAddress = IPAddress.Parse(userSecretsFixture[$"Elgato:{alias}:IPAddress"]);
			IPAddresses.Add(ipAddress);
			var physicalAddress = PhysicalAddress.Parse(userSecretsFixture[$"Elgato:{alias}:PhysicalAddress"]);
			PhysicalAddresses.Add(physicalAddress);
		}
	}

	public IConfiguration Configuration { get; }
	public Helpers.NetworkDiscoveryApi.Aliases Aliases { get; }
	public IList<IPAddress> IPAddresses { get; } = new List<IPAddress>();
	public IList<PhysicalAddress> PhysicalAddresses { get; } = new List<PhysicalAddress>();
}
