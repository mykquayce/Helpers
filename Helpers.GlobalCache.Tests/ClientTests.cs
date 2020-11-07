using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.GlobalCache.Tests
{
	public class ClientTests : IDisposable
	{
		private readonly IClient _sut;

		public ClientTests()
		{
			var config = new Concrete.Client.Config();
			_sut = new Concrete.Client(config);
		}

		public void Dispose() => _sut?.Dispose();

		[Fact]
		public async Task Discover_WithNoCancellationToken_RunsFor20Seconds_FindsAtLeastOneDevice()
		{
			var stopwatch = Stopwatch.StartNew();
			var beacons = await _sut.DiscoverAsync().ToListAsync();
			stopwatch.Stop();

			Assert.InRange(stopwatch.Elapsed.Seconds, 20, 21);

			Assert.NotNull(beacons);
			Assert.NotEmpty(beacons);
			Assert.All(beacons, Assert.NotNull);
		}

		[Theory]
		[InlineData(10_000)]
		public async Task Discover_FindsAtLeastOneDevice(int millisecondsDelay)
		{
			using var cts = new CancellationTokenSource(millisecondsDelay);

			var beacons = await _sut.DiscoverAsync(cts.Token).ToListAsync();

			Assert.NotNull(beacons);
			Assert.NotEmpty(beacons);
			Assert.All(beacons, Assert.NotNull);
		}

		[Theory]
		[InlineData("192.168.1.113:4998", "sendir,1:1,4,40192,1,1,96,24,48,24,24,24,48,24,24,24,48,24,24,24,24,24,24,24,24,24,24,24,24,24,48,24,48,24,24,24,24,4000\r")]
		public async Task SwitchOnTheAmp(string endPointString, string message)
		{
			var endPoint = IPEndPoint.Parse(endPointString);

			var response = await _sut.SendMessageAsync(endPoint, message);

			Assert.NotNull(response);
			Assert.StartsWith("completeir", response, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
