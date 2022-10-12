using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Helpers.SSH.Tests;

public class DependencyInjectionTests : IClassFixture<Fixtures.UserSecretsFixture>
{
	private readonly Config _config;

	public DependencyInjectionTests(Fixtures.UserSecretsFixture fixture)
	{
		_config = fixture.Config;
	}

	[Fact]
	public async Task Configuration()
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					[nameof(_config.Host)] = _config.Host,
					[nameof(_config.Port)] = _config.Port.ToString(),
					[nameof(_config.Username)] = _config.Username,
					[nameof(_config.Password)] = _config.Password,
					[nameof(_config.PathToPrivateKey)] = _config.PathToPrivateKey,
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.AddSSH(configuration)
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<Helpers.SSH.IService>();

		var leases = await sut.GetDhcpLeasesAsync().ToListAsync();

		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);

		if (serviceProvider is IAsyncDisposable disposable) await disposable.DisposeAsync();
	}

	[Fact]
	public async Task Config()
	{
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddSSH(_config)
			.BuildServiceProvider();

		var sut = serviceProvider.GetRequiredService<Helpers.SSH.IService>();

		var leases = await sut.GetDhcpLeasesAsync().ToListAsync();

		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);

		if (serviceProvider is IAsyncDisposable disposable) await disposable.DisposeAsync();
	}

	[Fact]
	public async Task Arguments()
	{
		IServiceProvider serviceProvider = new ServiceCollection()
			.AddSSH(_config.Host, _config.Port, _config.Username, _config.Password, _config.PathToPrivateKey)
			.BuildServiceProvider();

		var sut = serviceProvider.GetRequiredService<Helpers.SSH.IService>();

		var leases = await sut.GetDhcpLeasesAsync().ToListAsync();

		Assert.NotEmpty(leases);
		Assert.DoesNotContain(default, leases);

		if (serviceProvider is IAsyncDisposable disposable) await disposable.DisposeAsync();
	}
}
