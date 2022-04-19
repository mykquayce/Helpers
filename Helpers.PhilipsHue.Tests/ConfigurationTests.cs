using Microsoft.Extensions.Configuration;
using Xunit;

namespace Helpers.PhilipsHue.Tests;

public class ConfigurationTests
{
	public class POCO
	{
		public string? Key { get; set; }
	}

	[Theory]
	[InlineData("section", "key", "value")]
	public void AddValuestests(string section, string key, string value)
	{
		var json = @"{
  ""section"":
  {
	""key"": ""value""
  }
}";

		IConfigurationRoot config;
		{
			using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));

			//var dictionary = new Dictionary<string, string> { [$"{section}:{key}"] = value, };
			config = new ConfigurationBuilder()
				.AddJsonStream(stream)
				//.AddInMemoryCollection(dictionary)
				.Build();
		}

		Assert.NotNull(config[$"{section}:{key}"]);
		Assert.Equal(value, config[$"{section}:{key}"]);

		var configSection = config.GetSection(section);
		Assert.NotNull(configSection);
		//Assert.NotNull(configSection.Value);
		Assert.Equal(section, configSection.Key);
		Assert.NotNull(configSection[key]);
		Assert.NotEmpty(configSection[key]);
		Assert.Equal(value, configSection[key]);
		var actualValue = configSection.GetValue<string>(key);
		Assert.NotNull(actualValue);
		Assert.NotEmpty(actualValue);
		Assert.Equal(value, actualValue);

		var poco = configSection.Get<POCO>();
		Assert.NotNull(poco);
		Assert.NotNull(poco.Key);
		//Assert.NotNull(settings.Value);
		Assert.Equal(value, poco.Key);
	}

	[Theory]
	[InlineData("8391cb70-d94f-4863-b7e4-5659af167bc6")]
	public void UserSecrets(string id)
	{
		var config = new ConfigurationBuilder()
			.AddUserSecrets(id)
			.Build();

		Assert.NotNull(config);
		Assert.NotNull(config["PhilipsHue:BridgePhysicalAddress"]);
		Assert.NotNull(config["PhilipsHue:Username"]);

		var section = config.GetSection("PhilipsHue");

		Assert.NotNull(section);
		Assert.NotNull(section["BridgePhysicalAddress"]);
		Assert.NotNull(section["Username"]);
		Assert.NotEmpty(section["BridgePhysicalAddress"]);
		Assert.NotEmpty(section["Username"]);

		var poco = section.Get<Helpers.PhilipsHue.Clients.Concrete.PhilipsHueClient.Config>();

		Assert.NotNull(poco);
		Assert.NotNull(poco.BridgePhysicalAddress);
		Assert.NotNull(poco.Username);
		Assert.NotEmpty(poco.BridgePhysicalAddress!);
		Assert.NotEmpty(poco.Username);
	}
}
