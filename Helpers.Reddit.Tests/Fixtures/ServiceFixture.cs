namespace Helpers.Reddit.Tests.Fixtures;

public class ServiceFixture : ClientFixture
{
	public ServiceFixture()
	{
		Service = new Concrete.Service(base.Client);
	}

	public Helpers.Reddit.IService Service { get; }
}
