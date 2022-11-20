using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Tests;

[Collection(nameof(NonParallelCollection))]
public class DiscoveryClientTests : IClassFixture<Fixtures.Fixture>
{
	private readonly IDiscoveryClient _sut;

	public DiscoveryClientTests(Fixtures.Fixture fixture)
	{
		_sut = fixture.DiscoveryClient;
	}

	[Fact]
	public async Task DiscoveryTests()
	{
		// Act
		var devices = await _sut.DiscoverAsync().ToListAsync();

		// Assert
		Assert.NotEmpty(devices);
		Assert.DoesNotContain(default, devices);

		foreach (var (alias, ip, mac) in devices)
		{
			Assert.NotNull(alias);
			Assert.NotEmpty(alias);
			Assert.NotEqual(default, ip);
			Assert.NotEqual(IPAddress.None, ip);
			Assert.NotEqual(default, mac);
			Assert.NotEqual(PhysicalAddress.None, mac);
		}
	}

	public static IEnumerable<object[]> GetCancellationTokens()
	{
		yield return new object[1] { default(CancellationToken), };
		yield return new object[1] { CancellationToken.None, };
		yield return new object[1] { new CancellationToken(), };
	}

	[Theory]
	[MemberData(nameof(GetCancellationTokens))]
	public Task UncancellableCancellationToken_ThrowsException(CancellationToken cancellationToken)
	{
		var testCode = () => _sut.DiscoverAsync(cancellationToken).ToArrayAsync().AsTask();
		return Assert.ThrowsAsync<ArgumentException>(testCode);
	}
}
