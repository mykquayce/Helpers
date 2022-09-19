using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
	public class TPLinkServiceTests : IClassFixture<Fixtures.Fixture>
	{
		private readonly ITPLinkClient _client;
		private readonly ITPLinkService _service;

		public TPLinkServiceTests(Fixtures.Fixture fixture)
		{
			_client = fixture.Client;
			_service = fixture.Service;
		}

		[Fact]
		public async Task GetRealtimeDataTests()
		{
			double amps, watts, volts;
			var devices = await _client.DiscoverAsync().ToListAsync();

			Assert.NotEmpty(devices);

			foreach (var (alias, ip, mac) in devices)
			{
				(amps, watts, volts) = await _service.GetRealtimeDataAsync(alias);
				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);

				await Task.Delay(millisecondsDelay: 500);

				(amps, watts, volts) = await _service.GetRealtimeDataAsync(ip);
				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);

				await Task.Delay(millisecondsDelay: 500);

				(amps, watts, volts) = await _service.GetRealtimeDataAsync(mac);
				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);

				await Task.Delay(millisecondsDelay: 500);
			}
		}

		[Fact]
		public async Task GetSystemInfoTests()
		{
			string actualModel, actualAlias;
			PhysicalAddress actualMac;

			var devices = await _client.DiscoverAsync().ToListAsync();

			Assert.NotEmpty(devices);

			foreach (var (alias, ip, mac) in devices)
			{
				(actualModel, actualAlias, actualMac) = await _service.GetSystemInfoAsync(alias);
				Assert.NotNull(actualModel);
				Assert.NotEmpty(actualModel);
				Assert.Equal(alias, actualAlias);
				Assert.Equal(mac, actualMac);

				(actualModel, actualAlias, actualMac) = await _service.GetSystemInfoAsync(ip);
				Assert.NotNull(actualModel);
				Assert.NotEmpty(actualModel);
				Assert.Equal(alias, actualAlias);
				Assert.Equal(mac, actualMac);

				(actualModel, actualAlias, actualMac) = await _service.GetSystemInfoAsync(mac);
				Assert.NotNull(actualModel);
				Assert.NotEmpty(actualModel);
				Assert.Equal(alias, actualAlias);
				Assert.Equal(mac, actualMac);
			}
		}

		[Fact]
		public async Task GetStateTests()
		{
			var devices = await _client.DiscoverAsync().ToListAsync();

			Assert.NotEmpty(devices);

			foreach (var (alias, ip, mac) in devices)
			{
				await _service.GetStateAsync(alias);
				await _service.GetStateAsync(ip);
				await _service.GetStateAsync(mac);
			}
		}
	}
}
