﻿using System.Linq;
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
	}
}