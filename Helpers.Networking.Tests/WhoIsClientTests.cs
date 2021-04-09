using System;
using System.Collections.Generic;
using System.Linq;
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
				Assert.NotEmpty(ip!.ToString());
				Assert.InRange(mask ?? -1, 0, 64);
			}
		}

		[Theory]
		[InlineData(
			"% This is RIPE NCC's Routing Information Service\n% whois gateway to collected BGP Routing Tables, version2.0\n% IPv4 or IPv6 address to origin prefix match\n%\n% For more information visit http://www.ripe.net/ris/riswhois.html\n%\n% Connected to backend ris-whois12.ripe.net\n\n16509\t13.224.220.0/22\n\n\n",
			"16509\t13.224.220.0/22")]
		public void Parse(string input, params string[] expected)
		{
			var actual = input
				.Split(new char[2] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries)
				.Where(s => !s.StartsWith('%'))
				.ToList();

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(
			"% This is RIPE NCC's Routing Information Service\n% whois gateway to collected BGP Routing Tables, version2.0\n% IPv4 or IPv6 address to origin prefix match\n%\n% For more information visit http://www.ripe.net/ris/riswhois.html\n%\n% Connected to backend ris-whois11.ripe.net\n\nroute:        31.13.64.0/18\norigin:       AS32934\ndescr:        FACEBOOK, US\nlastupd-frst: 2021-03-19 09:04Z  91.206.52.68@rrc20\nlastupd-last: 2021-04-02 23:55Z  37.49.237.239@rrc21\nseen-at:      rrc00,rrc01,rrc03,rrc04,rrc05,rrc06,rrc07,rrc10,rrc11,rrc12,rrc13,rrc14,rrc15,rrc16,rrc18,rrc19,rrc20,rrc21,rrc22,rrc23,rrc24,rrc25\nnum-rispeers: 386\nsource:       RISWHOIS\n\n\n",
			"route", "origin", "descr", "lastupd-frst", "lastupd-last", "seen-at", "num-rispeers", "source")]
		/*[InlineData(
			"% This is RIPE NCC's Routing Information Service\n% whois gateway to collected BGP Routing Tables, version2.0\n% IPv4 or IPv6 address to origin prefix match\n%\n% For more information visit http://www.ripe.net/ris/riswhois.html\n%\n% Connected to backend ris-whois11.ripe.net\n\nroute:        31.13.64.0/18\norigin:       AS32934\ndescr:        FACEBOOK, US\nlastupd-frst: 2021-03-19 09:04Z  91.206.52.68@rrc20\nlastupd-last: 2021-04-02 23:55Z  37.49.237.239@rrc21\nseen-at:      rrc00,rrc01,rrc03,rrc04,rrc05,rrc06,rrc07,rrc10,rrc11,rrc12,rrc13,rrc14,rrc15,rrc16,rrc18,rrc19,rrc20,rrc21,rrc22,rrc23,rrc24,rrc25\nnum-rispeers: 386\nsource:       RISWHOIS\n\nroute:        31.13.72.0/24\norigin:       AS32934\ndescr:        FACEBOOK, US\nlastupd-frst: 2021-03-19 09:05Z  91.206.52.116@rrc20\nlastupd-last: 2021-04-02 18:27Z  80.81.192.231@rrc12\nseen-at:      rrc00,rrc01,rrc03,rrc04,rrc05,rrc06,rrc07,rrc10,rrc11,rrc12,rrc13,rrc14,rrc15,rrc16,rrc18,rrc19,rrc20,rrc21,rrc22,rrc23,rrc24,rrc25\nnum-rispeers: 332\nsource:       RISWHOIS\n\n\n",
			"route", "origin", "descr", "lastupd-frst", "lastupd-last", "seen-at", "num-rispeers", "source")]*/
		public void DeserializeToDictionary(string input, params string[] keys)
		{
			var dictionary = input
				.Split('\n', StringSplitOptions.RemoveEmptyEntries)
				.Where(s => !s.StartsWith('%'))
				.Select(line => Split(line, 14))
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.InvariantCultureIgnoreCase);

			Assert.NotNull(dictionary);
			Assert.NotEmpty(dictionary);

			foreach (var key in keys)
			{
				Assert.Contains(key, dictionary.Keys);
				var value = dictionary[key];
				Assert.NotNull(value);
				Assert.NotEmpty(value);
				Assert.Matches(@"^\w", value);
				Assert.Matches(@"\w$", value);
			}
		}

		[Theory]
		[InlineData("route:        31.13.64.0/18", "route", "31.13.64.0/18")]
		[InlineData("origin:       AS32934", "origin", "AS32934")]
		[InlineData("descr:        FACEBOOK, US", "descr", "FACEBOOK, US")]
		public void SplitTests(string line, string expectedKey, string expectedValue)
		{
			var (key, value) = Split(line, 14);
			Assert.Equal(expectedKey, key);
			Assert.Equal(expectedValue, value);
		}

		public static KeyValuePair<string, string> Split(string s, int column)
		{
			var key = s[..column].TrimEnd(' ')[..^1];
			var value = s[column..];
			var kvp = new KeyValuePair<string, string>(key, value);
			return kvp;
		}
	}
}
