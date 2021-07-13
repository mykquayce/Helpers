using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
	public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>
	{
		private readonly IService _sut;

		public ServiceTests(Fixtures.ServiceFixture fixture)
		{
			_sut = fixture.Service;
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task Power(string uuid, string message, string expected)
		{
			await _sut.ConnectAsync(uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40192,1,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeUp(string uuid, string message, string expected)
		{
			await _sut.ConnectAsync(uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40192,1,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeDown(string uuid, string message, string expected)
		{
			await _sut.ConnectAsync(uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r", "completeir,1:1,3\r")]
		public async Task Mute(string uuid, string message, string expected)
		{
			await _sut.ConnectAsync(uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("192.168.1.117")]
		public Task ConnectViaIP(string ipString)
		{
			var ip = IPAddress.Parse(ipString);
			return _sut.ConnectAsync(ip);
		}

		[Theory]
		[InlineData("192.168.1.117", "sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r")]
		public async Task SendMessageViaIP(string ipString, string message)
		{
			var ip = IPAddress.Parse(ipString);
			await _sut.ConnectAsync(ip);
			await _sut.SendAndReceiveAsync(message);
		}
	}
}
