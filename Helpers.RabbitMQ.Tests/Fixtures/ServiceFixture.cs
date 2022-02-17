using Microsoft.Extensions.Configuration;

namespace Helpers.RabbitMQ.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private const string _sectionName = "RabbitMQSettings";

	public ServiceFixture()
	{
		var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		var config = @base
			.Configuration.GetSection(_sectionName)
			.Get<Concrete.Service.Config>()
			?? throw new KeyNotFoundException($"{_sectionName} not found in user secrets");

		Service = new Concrete.Service(config);
	}

	public IService Service { get; }

	public void Dispose() => Service?.Dispose();
}
