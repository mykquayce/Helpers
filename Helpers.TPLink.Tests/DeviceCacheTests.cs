using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public class DeviceCacheTests
	{
		[Fact]
		public void Tests()
		{
			var cache = new Helpers.TPLink.Concrete.DeviceCache()
			{
				new ("device1", IPAddress.Parse("1.1.1.1"), PhysicalAddress.Parse("111111111111")),
				new ("device2", IPAddress.Parse("2.2.2.2"), PhysicalAddress.Parse("222222222222")),
				new ("device3", IPAddress.Parse("3.3.3.3"), PhysicalAddress.Parse("333333333333")),
			};

			Assert.True(cache.TryGetValue("device1", out var result1));
			Assert.NotNull(result1);

			Assert.True(cache.TryGetValue(IPAddress.Parse("1.1.1.1"), out var result2));
			Assert.NotNull(result2);

			Assert.Same(result1, result2);

			var count = 0;
			foreach (var (alias, ip, mac) in cache)
			{
				count++;
				Assert.NotNull(alias);
				Assert.NotNull(ip);
				Assert.NotNull(mac);
			}
			Assert.Equal(3, count);

			var dictionary1 = (IReadOnlyDictionary<string, Models.Device>)cache;

			Assert.NotNull(dictionary1);
			Assert.NotEmpty(dictionary1);
			Assert.Equal(3, dictionary1.Count);

			foreach (var (alias, device) in dictionary1)
			{
				Assert.NotNull(alias);
				Assert.NotNull(device);
			}
		}

		[Theory]
		[InlineData("alias")]
		public void AliasTests(string alias)
		{
			var cache = new Concrete.DeviceCache
			{
				new(alias, IPAddress.None, PhysicalAddress.None),
			};

			Assert.True(cache.TryGetValue(alias, out var result));
			Assert.NotNull(result);
			Assert.Equal(alias, result!.Alias);
		}

		[Theory]
		[InlineData("1.1.1.1")]
		public void IPAddressTests(string ip)
		{
			var cache = new Concrete.DeviceCache
			{
				new(string.Empty, IPAddress.Parse(ip), PhysicalAddress.None),
			};

			Assert.True(cache.TryGetValue(IPAddress.Parse(ip), out var result));
			Assert.NotNull(result);
			Assert.Equal(ip, result!.IPAddress.ToString().ToLowerInvariant());
		}

		[Theory]
		[InlineData("111111111111")]
		public void PhysicalAddressTests(string mac)
		{
			var cache = new Concrete.DeviceCache
			{
				new(string.Empty, IPAddress.None, PhysicalAddress.Parse(mac)),
			};

			Assert.True(cache.TryGetValue(PhysicalAddress.Parse(mac), out var result));
			Assert.NotNull(result);
			Assert.Equal(mac, result!.PhysicalAddress.ToString().ToLowerInvariant());
		}

		[Theory]
		[InlineData("alias", "1.1.1.1", "111111111111")]
		public void DoubleAdd(string alias, string ip, string mac)
		{
			var cache = new Concrete.DeviceCache
			{
				new(alias, IPAddress.Parse(ip), PhysicalAddress.Parse(mac)),
				new(alias, IPAddress.Parse(ip), PhysicalAddress.Parse(mac)),
			};

			Assert.Single(cache);
		}
	}
}
