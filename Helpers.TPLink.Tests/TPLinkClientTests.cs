using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
	public class TPLinkClientTests : IClassFixture<Fixtures.Fixture>
	{
		private readonly ITPLinkClient _sut;

		public TPLinkClientTests(Fixtures.Fixture fixture)
		{
			_sut = fixture.Client;
		}

		[Fact]
		public async Task DiscoverTests()
		{
			var localIPAddresses = Helpers.Networking.NetworkHelpers.GetAllBroadcastAddresses()
				.Select(ip => ip.Address)
				.ToList();

			var devices = await _sut.DiscoverAsync().ToListAsync();
			Assert.NotEmpty(devices);
			Assert.DoesNotContain(default, devices);

			foreach (var (alias, ip, mac) in devices)
			{
				Assert.NotNull(alias);
				Assert.NotNull(ip);
				Assert.DoesNotContain(ip, localIPAddresses);
				Assert.NotNull(mac);
			}
		}

		[Fact]
		public async Task GetRealtimeDataTests()
		{
			var devices = await _sut.DiscoverAsync().ToListAsync();

			foreach (var (_, ip, _) in devices)
			{
				var (amps, watts, volts) = await _sut.GetRealtimeDataAsync(ip);

				Assert.InRange(amps, .0001, double.MaxValue);
				Assert.InRange(watts, .0001, double.MaxValue);
				Assert.InRange(volts, .0001, double.MaxValue);
			}
		}

		[Theory]
		[InlineData("192.168.1.143")]
		public async Task GetSystemInfoTests(string ipString)
		{
			var ip = IPAddress.Parse(ipString);
			var result = await _sut.GetSystemInfoAsync(ip);
		}

		[Theory]
		[InlineData("192.168.1.143", false)]
		[InlineData("192.168.1.219", true)]
		[InlineData("192.168.1.248", false)]
		public async Task GetStateTests(string ipString, bool expected)
		{
			var ip = IPAddress.Parse(ipString);
			var actual = await _sut.GetStateAsync(ip);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("192.168.1.143", false)]
		[InlineData("192.168.1.143", true)]
		public async Task SetStateTests(string ipString, bool state)
		{
			var ip = IPAddress.Parse(ipString);
			await _sut.SetStateAsync(ip, state);
			await GetStateTests(ipString, state);
		}
	}
}
