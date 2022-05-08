using System.Net;

namespace Helpers.Elgato.Tests.Fixtures;

public class ConfigFixture
{
	private readonly XUnitClassFixtures.UserSecretsFixture _userSecretsFixture = new();

	public ConfigFixture()
	{
		KeylightIPAddress = IPAddress.Parse(_userSecretsFixture["Elgato:Keylight:IPAddress"]);
		LightstripIPAddress = IPAddress.Parse(_userSecretsFixture["Elgato:Lightstrip:IPAddress"]);
	}

	public IPAddress KeylightIPAddress { get; }
	public IPAddress LightstripIPAddress { get; }
}
