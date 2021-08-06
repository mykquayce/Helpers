using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
	public class SocketClientConnectionTests : IClassFixture<Fixtures.SocketClientFixture>
	{
		private readonly string _hostName;
		private readonly ushort _port;
		private readonly IPAddress _ipAddress;
		private readonly EndPoint _endPoint;
		private readonly Helpers.Networking.Clients.Concrete.SocketClient.Config _config = Helpers.Networking.Clients.Concrete.SocketClient.Config.Defaults;

		public SocketClientConnectionTests(Fixtures.SocketClientFixture fixture)
		{
			_hostName = fixture.HostName;
			_port = fixture.BroadcastPort;
			(_ipAddress, _) = Helpers.Networking.NetworkHelpers.PingAsync(_hostName).GetAwaiter().GetResult();
			_endPoint = new IPEndPoint(_ipAddress, _port);
		}

		[Fact]
		public async Task HostName()
		{
			using var sut = new Helpers.Networking.Clients.Concrete.SocketClient(_config);
			await sut.ConnectAsync(_hostName, _port);
		}

		[Fact]
		public async Task EndPoint()
		{
			using var sut = new Helpers.Networking.Clients.Concrete.SocketClient(_config);
			await sut.ConnectAsync(_endPoint);
		}

		[Fact]
		public async Task IPAddress()
		{
			using var sut = new Helpers.Networking.Clients.Concrete.SocketClient(_config);
			await sut.ConnectAsync(_ipAddress, _port);
		}
	}
}
