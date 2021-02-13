using System;
using System.Linq;
using System.Net.NetworkInformation;
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
		[InlineData("000c1e059cad")]
		public async Task Discover(string expected)
		{
			var actual = await _sut.DiscoverAsync().FirstAsync();

			Assert.Equal(expected, actual.ToString(), StringComparer.InvariantCultureIgnoreCase);
		}

		[Theory]
		[InlineData("000c1e059cad", "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task Power(string endPointString, string message)
		{
			await Discover(endPointString);
			var endPoint = PhysicalAddress.Parse(endPointString);
			await _sut.SendMessageasync(endPoint, message);
		}

		[Theory]
		[InlineData("000c1e059cad", "sendir,1:1,3,40192,1,1,96,24,24,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task VolumeUp(string endPointString, string message)
		{
			await Discover(endPointString);
			var endPoint = PhysicalAddress.Parse(endPointString);
			await _sut.SendMessageasync(endPoint, message);
		}

		[Theory]
		[InlineData("000c1e059cad", "sendir,1:1,4,40192,1,1,96,24,48,24,48,24,24,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task VolumeDown(string endPointString, string message)
		{
			await Discover(endPointString);
			var endPoint = PhysicalAddress.Parse(endPointString);
			await _sut.SendMessageasync(endPoint, message);
		}

		[Theory]
		[InlineData("000c1e059cad", "sendir,1:1,1,40064,1,1,96,24,24,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,897\r")]
		public async Task Mute(string endPointString, string message)
		{
			await Discover(endPointString);
			var endPoint = PhysicalAddress.Parse(endPointString);
			await _sut.SendMessageasync(endPoint, message);
		}
	}
}
