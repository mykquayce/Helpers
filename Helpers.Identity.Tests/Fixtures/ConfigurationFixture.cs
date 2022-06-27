using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Helpers.Identity.Tests.Fixtures;

public class ConfigurationFixture
{
	public ConfigurationFixture()
	{
		var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		Configuration = @base.Configuration.GetSection("identity");

		Authority = new Uri(Configuration["authority"]);
		ClientId = Configuration["clientid"];
		ClientSecret = Configuration["clientsecret"];
		Scope = Configuration["scope"];

		Config = new(Authority, ClientId, ClientSecret, Scope);

		OptionsConfig = Options.Create(Config);
	}

	public IConfiguration Configuration { get; }
	public Uri Authority { get; }
	public string ClientId { get; }
	public string ClientSecret { get; }
	public string Scope { get; }
	public Config Config { get; }
	public IOptions<Config> OptionsConfig { get; }
}
