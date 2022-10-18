using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.PhilipsHue.Tests.Fixtures;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var secrets = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		_serviceProvider = new ServiceCollection()
			.AddPhilipsHue(secrets.Configuration.GetSection("PhilipsHue"))
			.BuildServiceProvider();

		Config = _serviceProvider.GetRequiredService<IOptions<Config>>().Value;
		Client = _serviceProvider.GetRequiredService<IClient>();
		Service = _serviceProvider.GetRequiredService<IService>();
	}

	public Config Config { get; }
	public IClient Client { get; }
	public IService	Service { get; }

	public void Dispose() => (_serviceProvider as IDisposable)?.Dispose();
}
