using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	[Collection("Non-Parallel Collection")]
	public class UdpClientTests : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
	{
		private readonly Clients.IUdpClient _sut;

		public UdpClientTests(Helpers.XUnitClassFixtures.UserSecretsFixture userSecretsFixture)
		{
			var broadcastIPAddressString = userSecretsFixture["Networking:GlobalCache:BroadcastIPAddress"];
			var receivePort = ushort.Parse(userSecretsFixture["Networking:GlobalCache:ReceivePort"]);

			var config = new Helpers.Networking.Clients.Concrete.UdpClient.Config(broadcastIPAddressString, receivePort);
			_sut = new Helpers.Networking.Clients.Concrete.UdpClient(config);
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
