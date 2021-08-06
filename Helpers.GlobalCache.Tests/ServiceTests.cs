using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
	public class ServiceTests : IClassFixture<Fixtures.ServiceFixture>, IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
	{
		private readonly IService _sut;
		private readonly IPAddress _ipAddress;
		private readonly string _uuid;

		public ServiceTests(Fixtures.ServiceFixture serviceFixture, Helpers.XUnitClassFixtures.UserSecretsFixture userSecretsFixture)
		{
			_sut = serviceFixture.Service;
			_ipAddress = IPAddress.Parse(userSecretsFixture["GlobalCache:IPAddress"]);
			_uuid = userSecretsFixture["GlobalCache:UUID"];
		}

		[Theory]
		[InlineData("sendir,1:1,3,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task Power(string message, string expected)
		{
			await _sut.ConnectAsync(_uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("sendir,1:1,3,40192,1,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeUp(string message, string expected)
		{
			await _sut.ConnectAsync(_uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("sendir,1:1,3,40192,1,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r", "completeir,1:1,3\r")]
		public async Task VolumeDown(string message, string expected)
		{
			await _sut.ConnectAsync(_uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r", "completeir,1:1,3\r")]
		public async Task Mute(string message, string expected)
		{
			await _sut.ConnectAsync(_uuid);
			var response = await _sut.SendAndReceiveAsync(message);
			Assert.Equal(expected, response);
		}

		[Theory]
		[InlineData("sendir,1:1,3,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r")]
		public async Task SendMessageViaIP(string message)
		{
			await _sut.ConnectAsync(_ipAddress);
			await _sut.SendAndReceiveAsync(message);
		}
	}
}
