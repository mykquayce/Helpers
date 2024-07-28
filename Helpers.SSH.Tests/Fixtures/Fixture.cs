using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.SSH.Tests.Fixtures;

public sealed class Fixture : IDisposable
{
	private readonly IServiceProvider _serviceProvider;

	public Fixture()
	{
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<XUnitClassFixtures.UserSecretsFixture>()
			.Build();

		_serviceProvider = new ServiceCollection()
			.AddSshClient(builder =>
			{
				builder.Host = configuration["ssh:host"]!;
				builder.Port = ushort.Parse(configuration["ssh:port"]!);
				builder.Username = configuration["ssh:username"]!;
				builder.Password = configuration["ssh:password"];
				builder.PathToPrivateKey = configuration["ssh:pathtoprivatekey"];
			})
			.BuildServiceProvider();

		Client = _serviceProvider.GetRequiredService<IClient>();
		Library = _serviceProvider.GetRequiredService<Renci.SshNet.SshClient>();
		Service = _serviceProvider.GetRequiredService<IService>();
	}

	public IClient Client { get; }
	public Renci.SshNet.SshClient Library { get; }
	public IService Service { get; }

	public void Dispose() => (_serviceProvider as ServiceProvider)?.Dispose();
}
