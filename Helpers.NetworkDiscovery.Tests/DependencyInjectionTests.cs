using Helpers.NetworkDiscovery.Concrete;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.NetworkDiscovery.Tests;

public class DependencyInjectionTests
{
	[Fact]
	public async Task Test1()
	{
		// Arrange
		using var provider = new ServiceCollection()
			.AddTransient<HttpMessageHandler>(_ => new HttpClientHandler { AllowAutoRedirect = false, })
			.AddIdentityServerHandler(b =>
			{
				b.Authority = new Uri("https://identityserver/");
				b.ClientId = "client1";
				b.ClientSecret = "secret1";
			})
				.Services
			.AddHttpClient<IClient, Client>("NetworkDiscoveryClient", c => c.BaseAddress = new Uri("https://networkdiscovery/"))
				.ConfigurePrimaryHttpMessageHandler<HttpMessageHandler>()
				.AddHttpMessageHandler<IdentityServerHandler>()
				.Services
			.BuildServiceProvider();

		var sut = provider.GetRequiredService<IClient>();

		// Assert
		Assert.NotNull(sut);

		// Act
		var leases = await sut.GetAllLeasesAsync().ToArrayAsync();

		// Assert
		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);
	}
}
