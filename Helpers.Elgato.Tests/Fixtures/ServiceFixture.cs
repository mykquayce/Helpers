﻿using Microsoft.Extensions.Caching.Memory;
using System.Net.NetworkInformation;

namespace Helpers.Elgato.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly NetworkDiscoveryServiceFixture _networkDiscoveryServiceFixture = new();
	private readonly ClientFixture _elgatoClientFixture = new();

	public ServiceFixture()
	{
		Service = new Concrete.Service(
			_elgatoClientFixture.Client,
			_networkDiscoveryServiceFixture.Service);
	}

	public IService Service { get; }

	public void Dispose()
	{
		_elgatoClientFixture.Dispose();
		_networkDiscoveryServiceFixture.Dispose();
	}
}

public sealed class NetworkDiscoveryServiceFixture : IDisposable
{
	private readonly MemoryCacheFixture _memoryCacheFixture = new();
	private readonly NetworkDiscoveryClientFixture _networkDiscoveryClientFixture = new();

	public NetworkDiscoveryServiceFixture()
	{
		var configFixture = new ConfigFixture();
		var aliases = Helpers.NetworkDiscoveryApi.Aliases.Bind(configFixture.Configuration.GetSection("aliases"));

		Service = new Helpers.NetworkDiscoveryApi.Concrete.Service(
			_networkDiscoveryClientFixture.Client,
			_memoryCacheFixture.MemoryCache,
			aliases);
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
