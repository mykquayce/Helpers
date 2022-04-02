using System.Net;

namespace Helpers.GlobalCache.Tests;

[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
public class ServiceConnectTests : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
{
	private readonly string _hostName, _uuid;
	private readonly IPAddress _ipAddress;

	public ServiceConnectTests(Helpers.XUnitClassFixtures.UserSecretsFixture fixture)
	{
		_hostName = fixture["GlobalCache:HostName"];
		_ipAddress = IPAddress.Parse(fixture["GlobalCache:IPAddress"]);
		_uuid = fixture["GlobalCache:UUID"];
	}

	[Fact]
	public async Task IPAddressTest()
	{
		using IService sut = new Concrete.Service(Config.Defaults);
		await sut.ConnectAsync(_ipAddress);
	}

	[Fact]
	public async Task HostNameTest()
	{
		using IService sut = new Concrete.Service(Config.Defaults);
		await sut.ConnectAsync(_hostName);
	}

	[Fact]
	public async Task UuidTest()
	{
		using IService sut = new Concrete.Service(Config.Defaults);
		await sut.ConnectAsync(_uuid);
	}
}
