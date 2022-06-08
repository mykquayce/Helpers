using Microsoft.Extensions.Caching.Memory;

namespace Helpers.Elgato.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly AliasResolverServiceFixture _aliasResolverServiceFixture = new();
	private readonly ClientFixture _elgatoClientFixture = new();

	public ServiceFixture()
	{
		Service = new Concrete.Service(
			_elgatoClientFixture.Client,
			_aliasResolverServiceFixture.Service);
	}

	public IService Service { get; }

	public void Dispose()
	{
		_elgatoClientFixture.Dispose();
		_aliasResolverServiceFixture.Dispose();
	}
}

public sealed class AliasResolverServiceFixture : IDisposable
{
	private readonly ConfigFixture _configFixture = new();
	private readonly NetworkDiscoveryServiceFixture _networkDiscoveryServiceFixture = new();
	private readonly XUnitClassFixtures.UserSecretsFixture _userSecretsFixture = new();

	public AliasResolverServiceFixture()
	{
		var aliases = new Helpers.NetworkDiscoveryApi.Aliases();

		for (var a = 0; a < _configFixture.Aliases.Count; a++)
		{
			var alias = _configFixture.Aliases[a];
			var physicalAddressString = _configFixture.PhysicalAddresses[a].ToString().ToLowerInvariant();
			aliases.Add(alias, physicalAddressString);
		}

		Service = new Helpers.NetworkDiscoveryApi.Concrete.AliasResolverService(
			aliases,
			_networkDiscoveryServiceFixture.Service);
	}

	public Helpers.NetworkDiscoveryApi.IAliasResolverService Service { get; }

	public void Dispose()
	{
		_networkDiscoveryServiceFixture.Dispose();
	}
}

public sealed class NetworkDiscoveryServiceFixture : IDisposable
{
	private readonly MemoryCacheFixture _memoryCacheFixture = new();
	private readonly NetworkDiscoveryClientFixture _networkDiscoveryClientFixture = new();

	public NetworkDiscoveryServiceFixture()
	{
		Service = new Helpers.NetworkDiscoveryApi.Concrete.Service(
			_networkDiscoveryClientFixture.Client,
			_memoryCacheFixture.MemoryCache);
	}

	public Helpers.NetworkDiscoveryApi.IService Service { get; }

	public void Dispose()
	{
		_networkDiscoveryClientFixture.Dispose();
		_memoryCacheFixture.Dispose();
	}
}

public sealed class NetworkDiscoveryClientFixture : IDisposable
{
	private readonly IdentityClientFixture _identityClientFixture = new();
	private readonly XUnitClassFixtures.UserSecretsFixture _userSecretsFixture = new();
	private readonly HttpClient _httpClient;

	public NetworkDiscoveryClientFixture()
	{
		var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		var baseAddress = new Uri(_userSecretsFixture["NetworkDiscoveryApi"], UriKind.Absolute);
		_httpClient = new HttpClient(httpClientHandler) { BaseAddress = baseAddress, };
		Client = new Helpers.NetworkDiscoveryApi.Concrete.SecureClient(
			_httpClient,
			_identityClientFixture.Client);
	}

	public Helpers.NetworkDiscoveryApi.IClient Client { get; }

	public void Dispose()
	{
		_identityClientFixture.Dispose();
		_httpClient.Dispose();
	}
}

public sealed class IdentityClientFixture : IDisposable
{
	private readonly XUnitClassFixtures.UserSecretsFixture _userSecretsFixture = new();
	private readonly MemoryCacheFixture _memoryCacheFixture = new();
	private readonly HttpClient _httpClient;

	public IdentityClientFixture()
	{
		var httpClientHandler = new HttpClientHandler { AllowAutoRedirect = false, };
		var baseAddress = new Uri(_userSecretsFixture["identity:authority"], UriKind.Absolute);
		_httpClient = new HttpClient(httpClientHandler) { BaseAddress = baseAddress, };

		var config = new Helpers.Identity.Config(
			Authority: baseAddress,
			ClientId: _userSecretsFixture["identity:clientid"],
			ClientSecret: _userSecretsFixture["identity:clientsecret"],
			Scope: _userSecretsFixture["identity:scope"]);

		Client = new Helpers.Identity.Clients.Concrete.IdentityClient(
			config,
			_httpClient,
			_memoryCacheFixture.MemoryCache);
	}

	public Helpers.Identity.Clients.IIdentityClient Client { get; }

	public void Dispose()
	{
		_httpClient.Dispose();
		_memoryCacheFixture.Dispose();
	}
}

public sealed class MemoryCacheFixture : IDisposable
{
	public IMemoryCache MemoryCache { get; } = new MemoryCache(new MemoryCacheOptions());

	public void Dispose() => MemoryCache.Dispose();
}
