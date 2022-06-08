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

		Configuration.GetSection("Elgato:Aliases").Bind(Aliases);

		foreach (var alias in Aliases)
		{
			var ipAddress = IPAddress.Parse(userSecretsFixture[$"Elgato:{alias}:IPAddress"]);
			IPAddresses.Add(ipAddress);
			var physicalAddress = PhysicalAddress.Parse(userSecretsFixture[$"Elgato:{alias}:PhysicalAddress"]);
			PhysicalAddresses.Add(physicalAddress);
		}

	}

	public IConfiguration Configuration { get; }
	public IList<string> Aliases { get; } = new List<string>();
	public IList<IPAddress> IPAddresses { get; } = new List<IPAddress>();
	public IList<PhysicalAddress> PhysicalAddresses { get; } = new List<PhysicalAddress>();
}
