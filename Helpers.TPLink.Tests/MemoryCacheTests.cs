using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class MemoryCacheTests
{
	[Theory]
	[InlineData("vr front", "192.168.1.143", "28ee52eb0aa4")]
	public void Test1(string alias, string ipString, string macString)
	{
		IMemoryCache sut = new MemoryCache(new MemoryCacheOptions());
		{
			var expiration = DateTimeOffset.UtcNow.AddHours(1);
			var device = new Models.Device(alias, IPAddress.Parse(ipString), PhysicalAddress.Parse(macString));

			sut.Set(device.Alias, device, expiration);
			sut.Set(device.IPAddress, device, expiration);
			sut.Set(device.PhysicalAddress, device, expiration);
		}

		foreach (var key in new object[3] { alias, IPAddress.Parse(ipString), PhysicalAddress.Parse(macString), })
		{
			var found = sut.TryGetValue(key, out Models.Device? actual);

			Assert.True(found, userMessage: key + " not found");
			Assert.NotNull(actual);
			Assert.Equal(alias, actual.Alias);
			Assert.Equal(ipString, actual.IPAddress.ToString());
			Assert.Equal(macString, actual.PhysicalAddress.ToString(), StringComparer.OrdinalIgnoreCase);
		}
	}
}
