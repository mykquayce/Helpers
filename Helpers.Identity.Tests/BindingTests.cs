using Microsoft.Extensions.Configuration;

namespace Helpers.Identity.Tests;

public class BindingTests
{
	[Theory]
	[InlineData(
		"https://test-authority", "test-clientid", "test-clientsecret", "test-scope")]
	public void BindingRecordsTests(
		string authority, string clientId, string clientSecret, string scope)
	{
		// Arrange
		IConfiguration configuration;
		{
			var initialData = new Dictionary<string, string>
			{
				[nameof(authority)] = authority,
				[nameof(clientId)] = clientId,
				[nameof(clientSecret)] = clientSecret,
				[nameof(scope)] = scope,
			};

			configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();
		}

		// Act
		var config = Helpers.Identity.Config.Defaults;

		// Assert defaults are different
		Assert.NotEqual(authority, config.Authority.OriginalString);
		Assert.NotEqual(clientId, config.ClientId);
		Assert.NotEqual(clientSecret, config.ClientSecret);
		Assert.NotEqual(scope, config.Scope);

		// Act
		configuration.Bind(config);

		// Assert values are as specified
		Assert.Equal(authority, config.Authority.OriginalString);
		Assert.Equal(clientId, config.ClientId);
		Assert.Equal(clientSecret, config.ClientSecret);
		Assert.Equal(scope, config.Scope);
	}
}
