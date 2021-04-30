using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class TPLinkClientTests : IClassFixture<Fixtures.TPLinkWebClientFixture>
	{
		private readonly Helpers.TPLink.ITPLinkWebClient _sut;

		public TPLinkClientTests(Fixtures.TPLinkWebClientFixture tpLinkWebClientFixture)
		{
			_sut = tpLinkWebClientFixture.TPLinkWebClient;
		}

		[Theory]
		[InlineData("HS110(UK)", "amp")]
		public async Task EndToEnd(string deviceModel, string alias)
		{
			var devices = await _sut.GetDevicesAsync().ToListAsync();

			Assert.NotNull(devices);
			Assert.NotEmpty(devices);

			var device = (from d in devices
						  where string.Equals(d.deviceModel, deviceModel, StringComparison.InvariantCultureIgnoreCase)
						  where string.Equals(d.alias, alias, StringComparison.InvariantCultureIgnoreCase)
						  select d
						 ).SingleOrDefault();

			Assert.NotNull(device);
			Assert.NotNull(device!.deviceId);
			Assert.NotEmpty(device.deviceId);
			Assert.Matches("^[0-9A-Z]{40}$", device.deviceId);

			var data = await _sut.GetRealtimeDataAsync(device.deviceId!);

			Assert.NotNull(data);
			Assert.NotNull(data.power_mw);
			Assert.InRange(data.power_mw!.Value, 1, int.MaxValue);
		}

		[Theory]
		[InlineData("amp", "1.1.0 Build 201016 Rel.175140")]
		public async Task HasPlugUpdated(string alias, string expectedFwVer)
		{
			var device = await _sut.GetDevicesAsync().SingleOrDefaultAsync(d => d.alias == alias);

			Assert.NotNull(device);
			Assert.NotEqual(device!.fwVer, expectedFwVer, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
