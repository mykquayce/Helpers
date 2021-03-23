using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection("Non-Parallel Collection")]
	public class ServiceTests : IClassFixture<Fixtures.GlobalCacheServiceFixture>
	{
		private readonly Services.IGlobalCacheService _sut;

		public ServiceTests(Fixtures.GlobalCacheServiceFixture fixture)
		{
			_sut = fixture.GlobalCacheService;
		}

		[Theory]
		[InlineData("iTach059CAD", "sendir,1:1,3,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task Power(string hostName, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(hostName, message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("iTach059CAD", "sendir,1:1,3,40192,1,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeUp(string hostName, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(hostName, message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("iTach059CAD", "sendir,1:1,3,40192,1,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeDown(string hostName, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(hostName, message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("iTach059CAD", "sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r", "completeir,1:1,3\r")]
		public async Task Mute(string hostName, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(hostName, message);
			Assert.Equal(expected, response);
		}
	}
}
