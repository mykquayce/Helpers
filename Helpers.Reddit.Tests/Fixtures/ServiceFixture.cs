namespace Helpers.Reddit.Tests.Fixtures;

public sealed class ServiceFixture : IDisposable
{
	public ServiceFixture()
	{
		var clientFixture = new ClientFixture();
		var client = clientFixture.RedditClient;
		Service = new Concrete.Service(client);
	}

	public Helpers.Reddit.IService Service { get; }

	public void Dispose() => Service.Dispose();
}
