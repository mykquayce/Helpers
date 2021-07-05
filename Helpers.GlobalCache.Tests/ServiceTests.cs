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
			var response = await _sut.ConnectSendReceiveAsync(uuid, message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40192,1,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeUp(string uuid, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(uuid, message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40192,1,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeDown(string uuid, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(uuid, message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("GlobalCache_000C1E059CAD", "sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r", "completeir,1:1,3\r")]
		public async Task Mute(string uuid, string message, string expected)
		{
			var response = await _sut.ConnectSendReceiveAsync(uuid, message);
			Assert.Equal(expected, response);
		}
	}
}
