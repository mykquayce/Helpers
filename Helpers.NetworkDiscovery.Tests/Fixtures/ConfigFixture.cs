namespace Helpers.NetworkDiscovery.Tests.Fixtures;

public class ConfigFixture
{
	public ConfigFixture()
	{
		var secrets = new XUnitClassFixtures.UserSecretsFixture();
		IdentityConfig = secrets.GetSection<Identity.Config>(section: "identity");
		Config = secrets.GetSection<Config>(section: "networkdiscovery");
	}

	public Identity.Config IdentityConfig { get; }
	public Config Config { get; }
}
