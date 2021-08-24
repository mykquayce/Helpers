namespace Helpers.SSH.Tests.Fixtures;

public sealed class ServiceFixture : UserSecretsFixture, IDisposable
{
	public ServiceFixture()
	{
		Client = new Concrete.Client(base.Config);
		Service = new Concrete.Service(Client);
	}

	public IClient Client { get; }
	public IService Service { get; }

	public void Dispose() => Client.Dispose();
}
