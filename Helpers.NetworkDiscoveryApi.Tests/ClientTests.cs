using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.NetworkDiscoveryApi.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ClientTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly IClient _client;

	public ClientTests(Fixtures.ClientFixture fixture)
	{
		_client = fixture.Client;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "<Pending>")]
	[Theory(Skip = "requires unsecure third party")]
	[InlineData(5_000_000)]
	public async Task GetLeasesTests(int millisecondsDelay)
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

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "<Pending>")]
	[Theory(Skip = "requires unsecure third party")]
	[InlineData(5_000, "3C:6A:9D:14:D7:65")]
	public async Task GetLeaseTests(int millisecondsDelay, string physicalAddressString)
	{
		var physicalAddress = PhysicalAddress.Parse(physicalAddressString);

		Models.DhcpResponseObject lease;
		{
			using var cts = new CancellationTokenSource(millisecondsDelay);
			lease = await _client.GetLeaseAsync(physicalAddress, cts.Token);
		}

		Assert.NotNull(lease);
		var (expiration, ip, mac, hostName, identifier) = lease;

		Assert.NotEqual(default, expiration);
		Assert.NotEqual(default, mac);
		Assert.NotEqual(default, ip);
		Assert.NotEqual(string.Empty, hostName);
		Assert.NotEqual(string.Empty, identifier);

	}
}
