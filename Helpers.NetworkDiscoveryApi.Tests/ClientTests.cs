using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

public class ClientTests
{
	[Theory]
	[InlineData("http://localhost:34785")]
	public async Task DirectTests(string baseAddressString)
	{
		using var clientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var httpClient = new HttpClient(clientHandler) { BaseAddress = new Uri(baseAddressString), };

		IClient client = new Concrete.Client(httpClient);
		var leases = await client.GetLeasesAsync().ToListAsync();

		Assert.NotNull(leases);
		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);
		foreach (var (expiration, ip, mac, hostName, identifier) in leases)
		{
			Assert.NotEqual(default, expiration);
			Assert.NotEqual(default, mac);
			Assert.NotEqual(default, ip);
			Assert.NotEqual(string.Empty, hostName);
			Assert.NotEqual(string.Empty, identifier);
		}
	}


	[Theory]
	[InlineData("http://localhost:34785")]
	public async Task DependencyInjectionTests(string baseAddressString)
	{
		var serviceProvider = new ServiceCollection()
			.AddHttpClient<IClient, Concrete.Client>(httpClient =>
			{
				httpClient.BaseAddress = new Uri(baseAddressString);
			})
			.ConfigurePrimaryHttpMessageHandler(() =>
			{
				return new HttpClientHandler { AllowAutoRedirect = false, };
			})
			.Services
			.BuildServiceProvider();

		var client = serviceProvider.GetService<IClient>();

		Assert.NotNull(client);
		var leases = await client.GetLeasesAsync().ToListAsync();

		Assert.NotNull(leases);
		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);
		foreach (var (expiration, ip, mac, hostName, identifier) in leases)
		{
			Assert.NotEqual(default, expiration);
			Assert.NotEqual(default, mac);
			Assert.NotEqual(default, ip);
			Assert.NotEqual(string.Empty, hostName);
			Assert.NotEqual(string.Empty, identifier);
		}
	}
}
