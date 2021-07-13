using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
	public class UdpClientTests : IClassFixture<Fixtures.UdpClientFixture>
	{
		private readonly Clients.IUdpClient _sut;

		public UdpClientTests(Fixtures.UdpClientFixture udpClientFixture)
		{
			_sut = udpClientFixture.UdpClient;
		}

		[Fact]
		public async Task Discover()
		{
			var strings = await _sut.DiscoverAsync().ToListAsync();

			Assert.NotEmpty(strings);

			foreach (var @string in strings)
			{
				Assert.NotNull(@string);
				Assert.NotEmpty(@string);
			}
		}
	}
}
