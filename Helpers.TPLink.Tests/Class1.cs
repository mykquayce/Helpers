using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public sealed class Class1
	{
		[Theory]
		[InlineData("255.255.255.255", 20_002, "020000010000000000000000463cb5d3", "b09575e4f988")]
		public async Task GetRecentlyUpdatedForks(string host, int port, string hex, string expectedPhysicalAddress)
		{
			using var udpClient = new UdpClient();
			var message = hex.ToBytes().ToArray();
			await udpClient.SendAsync(message, message.Length, host, port);
			var result = await udpClient.ReceiveAsync();
			await using var stream = new MemoryStream(result.Buffer[16..]);
			var response = await JsonSerializer.DeserializeAsync<Helpers.TPLink.Models.DiscoveryResponseObject>(stream);
			Assert.NotNull(response);
			Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", response!.IPAddress.ToString());
			Assert.Equal(expectedPhysicalAddress, response!.PhysicalAddress.ToString(), StringComparer.InvariantCultureIgnoreCase);
		}

		[Theory]
		[InlineData("020000010000000000000000463cb5d3", 2, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 70, 60, 181, 211)]
		public void ToBytesTest(string before, params int[] expectedInts)
		{
			var expectedBytes = expectedInts.Select(i => (byte)i).ToArray();
			var actual = before.ToBytes().ToArray();

			Assert.NotNull(actual);
			Assert.NotEmpty(actual);
			Assert.Equal(expectedBytes, actual);
		}
	}

	public static class StringExtensions
	{
		public static IEnumerable<byte> ToBytes(this string hexesString)
		{
			using var enumerator = hexesString.GetEnumerator();

			while (enumerator.MoveNext())
			{
				var first = enumerator.Current;
				enumerator.MoveNext();
				var second = enumerator.Current;

				var chars = new[] { first, second, };
				var @string = new string(chars);
				var @byte = Convert.ToByte(@string, fromBase: 16);
				yield return @byte;
			}
		}
	}
}
