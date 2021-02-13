using Helpers.GlobalCache.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection("Non-Parallel Collection")]
	public class SocketClientTests : IClassFixture<Fixtures.SocketClientFixture>, IClassFixture<Fixtures.BeaconFixture>
	{
		private const ushort _port = 4_998;
		private readonly Clients.ISocketClient _sut;
		private readonly IReadOnlyList<Models.Beacon> _beacons;

		public SocketClientTests(Fixtures.SocketClientFixture socketClientFixture, Fixtures.BeaconFixture beaconsFixture)
		{
			_sut = socketClientFixture.SocketClient;

			_beacons = beaconsFixture.Beacons;
		}

		private EndPoint GetEndPoint(string physicalAddressString)
		{
			var physicalAddress = PhysicalAddress.Parse(physicalAddressString);
			var beacon = _beacons.Single(b => b.GetPhysicalAddress().Equals(physicalAddress));
			var endPoint = IPEndPoint.Parse($"{beacon.GetIPAddress()}:{_port:D}");
			return endPoint;
		}

		[Theory]
		[InlineData("000c1e059cad")]
		public ValueTask Connect(string physicalAddressString)
		{
			var endPoint = GetEndPoint(physicalAddressString);

			return _sut.ConnectAsync(endPoint);
		}

		[Theory]
		[InlineData("000c1e059cad", "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task Send(string physicalAddressString, string message)
		{
			await Connect(physicalAddressString);

			var tasks = Enumerable.Repeat(
				_sut.SendAsync(message).AsTask(),
				count: 5);

			var ints = await Task.WhenAll(tasks);

			Assert.All(ints, i => Assert.Equal(i, ints[0]));
		}

		[Theory]
		[InlineData("000c1e059cad", "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,4\r")]
		public async Task Receive(string physicalAddressString, string message, string expected)
		{
			await Send(physicalAddressString, message);
			var result = await _sut.ReceiveAsync();
			Assert.Equal((byte)'c', result[0]);
			var actual = Encoding.UTF8.GetString(result);
			Assert.Equal(expected, actual);
		}
	}
}
