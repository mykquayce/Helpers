﻿using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Networking.Tests
{
	public class WhoIsClientTests
	{
		[Theory]
		[InlineData(32_934)]
		[InlineData(7_224)]
		[InlineData(8_987)]
		[InlineData(14_618)]
		[InlineData(16_509)]
		public async Task GetIps(int asn)
		{
			var sut = new Helpers.Networking.Clients.Concrete.WhoIsClient();

			var subnetAddresses = await sut.GetIpsAsync(asn).ToListAsync();

			Assert.NotNull(subnetAddresses);
			Assert.NotEmpty(subnetAddresses);

			foreach (var (ip, mask) in subnetAddresses)
			{
				Assert.NotNull(ip);
				Assert.NotEmpty(ip.ToString());
				Assert.InRange(mask ?? -1, 0, 64);
			}
		}
	}
}
