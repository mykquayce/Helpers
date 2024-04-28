using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Nanoleaf.Tests;

public class DependencyInjectionTests
{
	[Fact]
	public void VariablesTests()
	{
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddNanoleaf(Constants.BaseAddress, Constants.Token)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
			.	Services
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}

	[Fact]
	public void ConfigTests()
	{
		var config = new Config { BaseAddress = Constants.BaseAddress, Token = Constants.Token, };

		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddNanoleaf(config)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.Services
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}

	private class Config : Concrete.Client.IConfig
	{
		public string Token { get; set; }
		public Uri BaseAddress { get; set; }
	}

	[Fact]
	public void ConfigurationTests()
	{
		var initialData = new Dictionary<string, string?>
		{
			[nameof(Constants.BaseAddress)] = Constants.BaseAddress.OriginalString,
			[nameof(Constants.Token)] = Constants.Token,
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(initialData)
			.Build();

		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddNanoleaf(configuration)
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.Services
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}

	[Fact]
	public void BuilderTests()
	{
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddNanoleaf(builder =>
			{
				builder.BaseAddress = Constants.BaseAddress;
				builder.Token = Constants.Token;
			})
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.Services
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}
}
