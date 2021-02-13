using System.Collections.Generic;
using System.Linq;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class BeaconFixture
	{
		public BeaconFixture()
		{
			using var udpClientFixture = new UdpClientFixture();

			Beacons = udpClientFixture.UdpClient.DiscoverAsync().ToListAsync().AsTask().GetAwaiter().GetResult();
		}

		public IReadOnlyList<Models.Beacon> Beacons { get; }
	}
}
