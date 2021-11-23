using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Xunit;

namespace Helpers.DockerHub.Tests;

public class ConfigTests
{
	[Theory]
	[InlineData(@"{
		""AuthApiBaseAddress"": ""https://auth.docker.io"",
		""RegistryApiBaseAddress"": ""https://registry-1.docker.io"",
		""Username"": ""username"",
		""Password"": ""password""
	}", "https://auth.docker.io", "https://registry-1.docker.io", "username", "password")]
	public void DeserializationTests(
		string json,
		string expectedAuthApiBaseAddress, string expectedRegistryApiBaseAddress, string expectedUsername, string expectedPassword)
	{
		var config = JsonSerializer.Deserialize<Config>(json);

		Assert.NotNull(config);
		Assert.Equal(expectedAuthApiBaseAddress, config.AuthApiBaseAddress.OriginalString);
		Assert.Equal(expectedRegistryApiBaseAddress, config.RegistryApiBaseAddress.OriginalString);
		Assert.Equal(expectedUsername, config.Username);
		Assert.Equal(expectedPassword, config.Password);
	}

	[Theory]
	[InlineData("https://auth.docker.io", "https://registry-1.docker.io", "username", "password")]
	public void ConfigurationTests(string authApiBaseAddress, string registryApiBaseAddress, string username, string password)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
				{
					["AuthApiBaseAddress"] = authApiBaseAddress,
					["RegistryApiBaseAddress"] = registryApiBaseAddress,
					["Username"] = username,
					["Password"] = password,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.JsonConfig<Config>(configuration)
				.BuildServiceProvider();
		}

		var options = serviceProvider.GetService<IOptions<Config>>();
		Assert.NotNull(options);

		var config = options.Value;
		Assert.NotNull(config);
		Assert.Equal(authApiBaseAddress, config.AuthApiBaseAddress.OriginalString);
		Assert.Equal(registryApiBaseAddress, config.RegistryApiBaseAddress.OriginalString);
		Assert.Equal(username, config.Username);
		Assert.Equal(password, config.Password);
	}
}
