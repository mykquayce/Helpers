using Helpers.Json.Converters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text.Json.Serialization;

namespace Helpers.Elgato.Tests.Fixtures
{
	public class ConfigFixture
	{
		public ConfigFixture()
		{
			var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			new ConfigurationBuilder()
				.AddConfiguration(@base.Configuration)
				.Build();

			var provider = new ServiceCollection()
				.JsonConfig<Addresses>(@base.Configuration.GetSection("Elgato"))
				.BuildServiceProvider();

			Addresses = provider.GetRequiredService<IOptions<Addresses>>().Value;
		}

		public Addresses Addresses { get; }
	}

	public record Addresses(
		[property: JsonConverter(typeof(JsonIPAddressConverter))] IPAddress IPAddress);
}
