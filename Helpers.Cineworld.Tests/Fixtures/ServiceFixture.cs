using Moq;
using System.Xml.Serialization;

namespace Helpers.Cineworld.Tests.Fixtures;

public class ServiceFixture
{
	private readonly static XmlSerializerFactory _xmlSerializerFactory = new();

	public ServiceFixture()
	{
		var allPerformances = Deserialize<Models.Generated.AllPerformances.cinemas>("Data", "all-performances.xml");
		var listings = Deserialize<Models.Generated.Listings.cinemas>("Data", "listings.xml");

		var clientMock = new Mock<IClient>();

		clientMock
			.Setup(c => c.GetAllPerformancesAsync(It.IsAny<CancellationToken?>()))
			.ReturnsAsync(allPerformances!);

		clientMock
			.Setup(c => c.GetListingsAsync(It.IsAny<CancellationToken?>()))
			.ReturnsAsync(listings!);

		Service = new Concrete.Service(clientMock.Object);
	}

	public IService Service { get; }

	private static T? Deserialize<T>(params string[] paths)
		where T : class
	{
		var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
		var path = Path.Combine(paths);
		using var stream = File.OpenRead(path);
		return serializer.Deserialize(stream) as T;
	}
}
