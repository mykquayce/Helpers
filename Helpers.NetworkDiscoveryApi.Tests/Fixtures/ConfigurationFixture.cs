namespace Helpers.NetworkDiscoveryApi.Tests.Fixtures;

public class ConfigurationFixture
{
	public ConfigurationFixture()
	{
		var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		BaseAddress = new Uri(@base["NetworkDiscoveryApi"]);
		Authority = new Uri(@base["Identity:Authority"]);
		ClientId = @base["Identity:ClientId"];
		ClientSecret = @base["Identity:ClientSecret"];
		Scope = @base["Identity:Scope"];
	}

	public Uri BaseAddress { get; }
	public Uri Authority { get; }
	public string ClientId { get; }
	public string ClientSecret { get; }
	public string Scope { get; }
}
