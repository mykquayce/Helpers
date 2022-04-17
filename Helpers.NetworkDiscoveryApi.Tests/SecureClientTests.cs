using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class SecureClientTests : IClassFixture<Fixtures.SecureClientFixture>
{
	private readonly IClient _client;

	public SecureClientTests(Fixtures.SecureClientFixture fixture)
	{
		_client = fixture.Client;
	}

	[Theory]
	[InlineData(5_000)]
	public async Task Test1(int millisecondsDelay)
	{
		ICollection<Models.DhcpResponseObject> leases;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay);
			leases = await _client.GetLeasesAsync(cts.Token).ToListAsync(cts.Token);
		}

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
