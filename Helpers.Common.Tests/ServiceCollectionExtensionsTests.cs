using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Helpers.Common.Tests;

public class ServiceCollectionExtensionsTests
{
	[Theory]
	[InlineData(default, default, default)]
	[InlineData("alice", default, "alice")]
	[InlineData(default, "./data/username.txt", "bob")]
	[InlineData("alice", "./data/username.txt", "alice")]
	public void BindObject_FromFile(string? username, string? usernameFile, string? expected)
	{
		// Arrange
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					["Username"] = username,
					["Username_File"] = usernameFile,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.FileConfigure<Config>(configuration)
				.BuildServiceProvider();

			// Assert : old configuration hasn't changed
			Assert.Equal(username, configuration["Username"]);
		}

		// Act
		var options = serviceProvider.GetService<IOptions<Config>>();

		// Assert
		Assert.NotNull(options);
		Assert.NotNull(options.Value);
		Assert.Equal(expected, options.Value.Username);
	}

	public record Config(string? Username)
	{
		public Config() : this(default(string)) { }
	}
}
