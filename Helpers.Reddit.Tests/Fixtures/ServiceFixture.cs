using Microsoft.Extensions.Options;

namespace Helpers.Reddit.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	private readonly ClientFixture _clientFixture = new();

	public ServiceFixture()
	{
		var client = _clientFixture.Client;
		Service = new Concrete.Service(client);
	}

	public IService Service { get; }

	public void Dispose() => _clientFixture.Dispose();
}
