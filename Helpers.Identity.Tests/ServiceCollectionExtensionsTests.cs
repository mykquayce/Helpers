using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.Identity.Tests;

public class ServiceCollectionExtensionsTests : IClassFixture<Fixtures.ConfigurationFixture>
{
	private const string _pattern = @"^[-0-9A-Z_a-z]{160}\.[-0-9A-Z_a-z]{200,300}\.[-0-9A-Z_a-z]{300,400}$";
	private readonly IConfiguration _configuration;
	private readonly Config _config;
	private readonly IOptions<Config> _optionsConfig;
	private readonly Uri _authority;
	private readonly string _clientId, _clientSecret, _scope;

	public ServiceCollectionExtensionsTests(Fixtures.ConfigurationFixture configurationFixture)
	{
		_configuration = configurationFixture.Configuration;
		_config = configurationFixture.Config;
		_optionsConfig = configurationFixture.OptionsConfig;
		_authority = configurationFixture.Authority;
		_clientId = configurationFixture.ClientId;
		_clientSecret = configurationFixture.ClientSecret;
		_scope = configurationFixture.Scope;
	}

	[Fact]
	public async Task GetAccessTokenTests_Configuration()
	{
		// Arrange
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddIdentityClient(_configuration)
			.BuildServiceProvider();

		var sut = serviceProvider.GetRequiredService<Helpers.Identity.Clients.IIdentityClient>();

		// Act
		var token = await sut.GetAccessTokenAsync();

		// Assert
		Assert.NotNull(token);
		Assert.NotEmpty(token);
		Assert.Matches(_pattern, token);
	}

	[Fact]
	public async Task GetAccessTokenTests_Config()
	{
		// Arrange
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddIdentityClient(_config)
			.BuildServiceProvider();

		var sut = serviceProvider.GetRequiredService<Helpers.Identity.Clients.IIdentityClient>();

		// Act
		var token = await sut.GetAccessTokenAsync();

		// Assert
		Assert.NotNull(token);
		Assert.NotEmpty(token);
		Assert.Matches(_pattern, token);
	}

	[Fact]
	public async Task GetAccessTokenTests_OptionsConfig()
	{
		// Arrange
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddIdentityClient(_optionsConfig)
			.BuildServiceProvider();

		var sut = serviceProvider.GetRequiredService<Helpers.Identity.Clients.IIdentityClient>();

		// Act
		var token = await sut.GetAccessTokenAsync();

		// Assert
		Assert.NotNull(token);
		Assert.NotEmpty(token);
		Assert.Matches(_pattern, token);
	}

	[Fact]
	public async Task GetAccessTokenTests_Values()
	{
		// Arrange
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddIdentityClient(_authority, _clientId, _clientSecret, _scope)
			.BuildServiceProvider();

		var sut = serviceProvider.GetRequiredService<Helpers.Identity.Clients.IIdentityClient>();

		// Act
		var token = await sut.GetAccessTokenAsync();

		// Assert
		Assert.NotNull(token);
		Assert.NotEmpty(token);
		Assert.Matches(_pattern, token);
	}
}
