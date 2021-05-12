using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.Elgato.Tests
{
	public class CachingServiceTests
	{
		[Fact]
		public void Tests()
		{
			var sut = new Concrete.AddressesCache();

			sut.Set(PhysicalAddress.Parse("1800db0e980d"), IPAddress.Parse("192.168.1.180"), DateTimeOffset.MaxValue);
			sut.Set(PhysicalAddress.Parse("ccf735b2a678"), IPAddress.Parse("192.168.1.179"), DateTimeOffset.MaxValue);
			sut.Set(PhysicalAddress.Parse("000c1e059cad"), IPAddress.Parse("192.168.1.121"), DateTimeOffset.MaxValue);

			bool ok;
			IPAddress? actual;

			ok = sut.TryGet(PhysicalAddress.Parse("1800db0e980d"), out actual);

			Assert.True(ok);
			Assert.NotNull(actual);
			Assert.Equal("192.168.1.180", actual!.ToString());

			ok = sut.TryGet(PhysicalAddress.Parse("ccf735b2a678"), out actual);

			Assert.True(ok);
			Assert.NotNull(actual);
			Assert.Equal("192.168.1.179", actual!.ToString());

			ok = sut.TryGet(PhysicalAddress.Parse("000c1e059cad"), out actual);

			Assert.True(ok);
			Assert.NotNull(actual);
			Assert.Equal("192.168.1.121", actual!.ToString());
		}

		[Fact]
		public void Dictionary()
		{
			IDictionary<PhysicalAddress, IPAddress> sut = new Dictionary<PhysicalAddress, IPAddress>()
			{
				[PhysicalAddress.Parse("1800db0e980d")] = IPAddress.Parse("192.168.1.180"),
				[PhysicalAddress.Parse("ccf735b2a678")] = IPAddress.Parse("192.168.1.179"),
				[PhysicalAddress.Parse("000c1e059cad")] = IPAddress.Parse("192.168.1.121"),
			};

			Assert.Equal("192.168.1.180", sut[PhysicalAddress.Parse("1800db0e980d")].ToString());
			Assert.Equal("192.168.1.179", sut[PhysicalAddress.Parse("ccf735b2a678")].ToString());
			Assert.Equal("192.168.1.121", sut[PhysicalAddress.Parse("000c1e059cad")].ToString());
		}
	}
}
