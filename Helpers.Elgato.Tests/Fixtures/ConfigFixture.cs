using Helpers.Json.Converters;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json.Serialization;

namespace Helpers.Elgato.Tests.Fixtures
{
	public class ConfigFixture
	{
		public ConfigFixture()
		{
			var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();
			Addresses = @base.Configuration.GetSection("Elgato").GetSection("EndPoints").JsonConfig<Addresses>();
		}

		public Addresses Addresses { get; }
	}

	public record Addresses(
		[property: JsonConverter(typeof(JsonIPAddressConverter))] IPAddress IPAddress);
}
