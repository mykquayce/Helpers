using Microsoft.Extensions.Configuration;
using System.Net;

namespace Helpers.Elgato.Tests.Fixtures;

public class ConfigFixture
{
	public ConfigFixture()
	{
		var userSecretsFixture = new XUnitClassFixtures.UserSecretsFixture();

		Configuration = userSecretsFixture.Configuration;

		IPAddresses = Configuration.GetSection("elgato:ipaddresses")
			.Get<string[]>()!
			.Select(IPAddress.Parse)
			.ToArray()
			.AsReadOnly();
	}

	public IConfiguration Configuration { get; }
	public IReadOnlyCollection<IPAddress> IPAddresses { get; } = new List<IPAddress>();
}
