﻿using Helpers.Networking.Models;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using Xunit;

namespace Helpers.SSH.Tests;

public class ServiceTests(Fixtures.Fixture fixture) : IClassFixture<Fixtures.Fixture>
{
	private readonly IService _sut = fixture.Service;

	[Theory]
	[InlineData("77.68.0.0/17")]
	[InlineData("77.68.11.211")]
	public async Task AddBlackhole(string s)
	{
		var prefix = Networking.Models.AddressPrefix.Parse(s, null);

		// see if already exists
		var exists = await _sut.GetBlackholesAsync().AnyAsync(b => b == prefix);

		// if so, remove it
		if (exists) await _sut.DeleteBlackholeAsync(prefix);

		// Assert it was removed
		Assert.False(await _sut.GetBlackholesAsync().AnyAsync(b => b == prefix));

		// add it
		await _sut.AddBlackholeAsync(prefix);

		await Task.Delay(millisecondsDelay: 500);

		var a = await _sut.GetBlackholesAsync().ToListAsync();

		// Assert it was added
		Assert.True(await _sut.GetBlackholesAsync().AnyAsync(b => b == prefix));

		// delete it
		await _sut.DeleteBlackholeAsync(prefix);

		// Assert it was deleted
		Assert.False(await _sut.GetBlackholesAsync().AnyAsync(b => b == prefix));

		// if it existed previously, put it back
		if (exists) await _sut.AddBlackholeAsync(prefix);
	}

	[Theory]
	[InlineData("77.68.11.211", "77.68.11.211")]
	public void Equality(string leftString, string rightString)
	{
		Helpers.Networking.Models.AddressPrefix
			left = Networking.Models.AddressPrefix.Parse(leftString, null),
			right = Networking.Models.AddressPrefix.Parse(rightString, null);
		Assert.False(object.ReferenceEquals(left, right));
		Assert.True(left == right);
		Assert.True(object.Equals(left, right));
		Assert.Equal(left, right);
	}

	[Theory]
	[InlineData("77.68.0.0/17", "77.68.0.0/17")]
	[InlineData("77.68.11.211", "77.68.11.211")]
	public void String(string before, string expected) => Assert.Equal(expected, before.ToString());

	[Fact]
	public async Task GetBlackholes()
	{
		var results = await _sut.GetBlackholesAsync().ToListAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.DoesNotContain(default, results);

		foreach (var (ip, mask) in results)
		{
			Assert.NotNull(ip);
			Assert.NotEqual(0, mask);
		}
	}

	[Theory]
	[InlineData("31.13.24.0", System.Net.Sockets.AddressFamily.InterNetwork)]
	[InlineData("2620:0:1c00::", System.Net.Sockets.AddressFamily.InterNetworkV6)]
	public void IPAddressVersionTest(string ipAddressString, System.Net.Sockets.AddressFamily expected)
	{
		var ok = IPAddress.TryParse(ipAddressString, out var ipAddress);

		Assert.True(ok);
		Assert.NotNull(ipAddress);
		Assert.Equal(expected, ipAddress!.AddressFamily);
	}

	[Fact]
	public async Task GetDhcpLeases()
	{
		DateTime now = DateTime.UtcNow, later = now.AddDays(7);
		var entries = await _sut.GetDhcpLeasesAsync().ToListAsync();

		Assert.NotNull(entries);
		Assert.NotEmpty(entries);

		foreach (var (expiration, physicalAddress, ipAddress, hostName, identifier) in entries)
		{
			Assert.Equal(DateTimeKind.Utc, expiration.Kind);
			Assert.InRange(expiration, now, later);
			Assert.NotNull(physicalAddress);
			Assert.NotNull(ipAddress);
			if (hostName is not null) Assert.NotEmpty(hostName);
			if (identifier is not null) Assert.NotEmpty(identifier);
		}
	}

	[Theory]
	[InlineData("192.168.1.217", "3c6a9d14d765")]
	public async Task GetLeaseByIPAddress(string ipAddressString, string physicalAddressString)
	{
		var ipAddress = IPAddress.Parse(ipAddressString);
		var lease = await _sut.GetLeaseByIPAddressAsync(ipAddress);
		Assert.Equal(physicalAddressString, lease.PhysicalAddress.ToString(), StringComparer.InvariantCultureIgnoreCase);
	}

	[Theory]
	[InlineData("3c6a9d14d765", "192.168.1.217")]
	public async Task GetLeaseByPhysicalAddress(string physicalAddressString, string ipAddressString)
	{
		var physicalAddress = PhysicalAddress.Parse(physicalAddressString);
		var lease = await _sut.GetLeaseByPhysicalAddressAsync(physicalAddress);
		Assert.Equal(ipAddressString, lease.IPAddress.ToString(), StringComparer.InvariantCultureIgnoreCase);
	}

	#region destructive tests
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable xUnit1004 // Test methods should not be skipped
	[Fact(Skip = "removes all the blackholes")]
	public async Task RemoveAllBlackholes()
	{
		await foreach (var subnetAddress in _sut.GetBlackholesAsync())
		{
			await _sut.DeleteBlackholeAsync(subnetAddress);
		}
	}

	[Theory(Skip = "blacklists facebook")]
	[InlineData(
		"31.13.24.0/21,31.13.64.0/18,31.13.64.0/19,31.13.64.0/24,31.13.65.0/24,31.13.66.0/24,31.13.67.0/24,31.13.68.0/24,31.13.69.0/24,31.13.70.0/24,31.13.71.0/24,31.13.72.0/24,31.13.73.0/24,31.13.74.0/24,31.13.75.0/24,31.13.76.0/24,31.13.77.0/24,31.13.80.0/24,31.13.81.0/24,31.13.82.0/24,31.13.83.0/24,31.13.84.0/24,31.13.85.0/24,31.13.86.0/24,31.13.87.0/24,31.13.88.0/24,31.13.89.0/24,31.13.92.0/24,31.13.93.0/24,31.13.94.0/24,31.13.96.0/19,45.64.40.0/22,66.220.144.0/20,66.220.144.0/21,66.220.152.0/21,69.63.176.0/20,69.63.176.0/21,69.63.184.0/21,69.171.224.0/19,69.171.224.0/20,69.171.240.0/20,69.171.250.0/24,74.119.76.0/22,102.132.96.0/20,102.132.96.0/24,102.132.99.0/24,103.4.96.0/22,129.134.0.0/17,129.134.25.0/24,129.134.26.0/24,129.134.27.0/24,129.134.28.0/24,129.134.29.0/24,129.134.30.0/23,129.134.30.0/24,129.134.31.0/24,129.134.64.0/24,129.134.65.0/24,129.134.66.0/24,129.134.67.0/24,129.134.68.0/24,129.134.69.0/24,129.134.70.0/24,129.134.71.0/24,129.134.72.0/24,129.134.73.0/24,129.134.74.0/24,129.134.127.0/24,157.240.0.0/17,157.240.0.0/24,157.240.1.0/24,157.240.2.0/24,157.240.3.0/24,157.240.6.0/24,157.240.7.0/24,157.240.8.0/24,157.240.9.0/24,157.240.10.0/24,157.240.11.0/24,157.240.12.0/24,157.240.13.0/24,157.240.14.0/24,157.240.15.0/24,157.240.17.0/24,157.240.18.0/24,157.240.19.0/24,157.240.20.0/24,157.240.21.0/24,157.240.22.0/24,157.240.24.0/24,157.240.26.0/24,157.240.27.0/24,157.240.28.0/24,157.240.29.0/24,157.240.30.0/24,157.240.31.0/24,157.240.192.0/18,157.240.193.0/24,157.240.194.0/24,157.240.195.0/24,157.240.196.0/24,157.240.197.0/24,157.240.199.0/24,157.240.200.0/24,157.240.201.0/24,157.240.204.0/24,157.240.205.0/24,157.240.206.0/24,157.240.209.0/24,157.240.210.0/24,157.240.211.0/24,157.240.212.0/24,157.240.213.0/24,157.240.214.0/24,157.240.215.0/24,157.240.216.0/24,157.240.217.0/24,157.240.218.0/24,157.240.219.0/24,157.240.220.0/24,157.240.221.0/24,157.240.222.0/24,157.240.223.0/24,157.240.224.0/24,173.252.64.0/19,173.252.88.0/21,173.252.96.0/19,179.60.192.0/22,179.60.192.0/24,179.60.194.0/24,179.60.195.0/24,185.60.216.0/22,185.60.216.0/24,185.60.217.0/24,185.60.218.0/24,185.60.219.0/24,185.89.218.0/23,185.89.218.0/24,185.89.219.0/24,204.15.20.0/22,2620:0:1c00::/40,2a03:2880::/32,2a03:2880::/36,2a03:2880:1000::/36,2a03:2880:2000::/36,2a03:2880:3000::/36,2a03:2880:f001::/48,2a03:2880:f003::/48,2a03:2880:f004::/48,2a03:2880:f005::/48,2a03:2880:f006::/48,2a03:2880:f007::/48,2a03:2880:f008::/48,2a03:2880:f00a::/48,2a03:2880:f00c::/48,2a03:2880:f00d::/48,2a03:2880:f00e::/48,2a03:2880:f00f::/48,2a03:2880:f010::/48,2a03:2880:f011::/48,2a03:2880:f012::/48,2a03:2880:f013::/48,2a03:2880:f016::/48,2a03:2880:f017::/48,2a03:2880:f019::/48,2a03:2880:f01c::/48,2a03:2880:f01f::/48,2a03:2880:f021::/48,2a03:2880:f023::/48,2a03:2880:f024::/48,2a03:2880:f027::/48,2a03:2880:f028::/48,2a03:2880:f029::/48,2a03:2880:f02a::/48,2a03:2880:f02b::/48,2a03:2880:f02c::/48,2a03:2880:f02d::/48,2a03:2880:f02f::/48,2a03:2880:f030::/48,2a03:2880:f031::/48,2a03:2880:f032::/48,2a03:2880:f033::/48,2a03:2880:f034::/48,2a03:2880:f035::/48,2a03:2880:f036::/48,2a03:2880:f037::/48,2a03:2880:f038::/48,2a03:2880:f03a::/48,2a03:2880:f03d::/48,2a03:2880:f03e::/48,2a03:2880:f03f::/48,2a03:2880:f040::/48,2a03:2880:f041::/48,2a03:2880:f042::/48,2a03:2880:f043::/48,2a03:2880:f044::/48,2a03:2880:f045::/48,2a03:2880:f047::/48,2a03:2880:f048::/48,2a03:2880:f04a::/48,2a03:2880:f04b::/48,2a03:2880:f04e::/48,2a03:2880:f04f::/48,2a03:2880:f050::/48,2a03:2880:f052::/48,2a03:2880:f053::/48,2a03:2880:f054::/48,2a03:2880:f056::/48,2a03:2880:f057::/48,2a03:2880:f058::/48,2a03:2880:f059::/48,2a03:2880:f05a::/48,2a03:2880:f05b::/48,2a03:2880:f05c::/48,2a03:2880:f05e::/48,2a03:2880:f060::/48,2a03:2880:f065::/48,2a03:2880:f066::/48,2a03:2880:f067::/48,2a03:2880:f068::/48,2a03:2880:f0fc::/47,2a03:2880:f0fc::/48,2a03:2880:f0fd::/48,2a03:2880:f0ff::/48,2a03:2880:f101::/48,2a03:2880:f103::/48,2a03:2880:f104::/48,2a03:2880:f105::/48,2a03:2880:f106::/48,2a03:2880:f107::/48,2a03:2880:f108::/48,2a03:2880:f10a::/48,2a03:2880:f10c::/48,2a03:2880:f10d::/48,2a03:2880:f10e::/48,2a03:2880:f10f::/48,2a03:2880:f110::/48,2a03:2880:f111::/48,2a03:2880:f112::/48,2a03:2880:f113::/48,2a03:2880:f116::/48,2a03:2880:f117::/48,2a03:2880:f119::/48,2a03:2880:f11c::/48,2a03:2880:f11f::/48,2a03:2880:f121::/48,2a03:2880:f123::/48,2a03:2880:f124::/48,2a03:2880:f127::/48,2a03:2880:f128::/48,2a03:2880:f129::/48,2a03:2880:f12a::/48,2a03:2880:f12b::/48,2a03:2880:f12c::/48,2a03:2880:f12d::/48,2a03:2880:f12f::/48,2a03:2880:f130::/48,2a03:2880:f131::/48,2a03:2880:f132::/48,2a03:2880:f133::/48,2a03:2880:f134::/48,2a03:2880:f135::/48,2a03:2880:f136::/48,2a03:2880:f137::/48,2a03:2880:f138::/48,2a03:2880:f13a::/48,2a03:2880:f13d::/48,2a03:2880:f13e::/48,2a03:2880:f13f::/48,2a03:2880:f140::/48,2a03:2880:f141::/48,2a03:2880:f142::/48,2a03:2880:f143::/48,2a03:2880:f144::/48,2a03:2880:f145::/48,2a03:2880:f147::/48,2a03:2880:f148::/48,2a03:2880:f14a::/48,2a03:2880:f14b::/48,2a03:2880:f14e::/48,2a03:2880:f14f::/48,2a03:2880:f150::/48,2a03:2880:f152::/48,2a03:2880:f153::/48,2a03:2880:f154::/48,2a03:2880:f156::/48,2a03:2880:f157::/48,2a03:2880:f158::/48,2a03:2880:f159::/48,2a03:2880:f15a::/48,2a03:2880:f15b::/48,2a03:2880:f15c::/48,2a03:2880:f15e::/48,2a03:2880:f160::/48,2a03:2880:f162::/48,2a03:2880:f163::/48,2a03:2880:f164::/48,2a03:2880:f165::/48,2a03:2880:f1fc::/47,2a03:2880:f1fc::/48,2a03:2880:f1fd::/48,2a03:2880:f1ff::/48,2a03:2880:f201::/48,2a03:2880:f203::/48,2a03:2880:f204::/48,2a03:2880:f205::/48,2a03:2880:f206::/48,2a03:2880:f207::/48,2a03:2880:f208::/48,2a03:2880:f20a::/48,2a03:2880:f20c::/48,2a03:2880:f20d::/48,2a03:2880:f20e::/48,2a03:2880:f20f::/48,2a03:2880:f210::/48,2a03:2880:f211::/48,2a03:2880:f212::/48,2a03:2880:f213::/48,2a03:2880:f216::/48,2a03:2880:f217::/48,2a03:2880:f219::/48,2a03:2880:f21c::/48,2a03:2880:f21f::/48,2a03:2880:f221::/48,2a03:2880:f223::/48,2a03:2880:f224::/48,2a03:2880:f227::/48,2a03:2880:f228::/48,2a03:2880:f229::/48,2a03:2880:f22a::/48,2a03:2880:f22b::/48,2a03:2880:f22c::/48,2a03:2880:f22d::/48,2a03:2880:f22f::/48,2a03:2880:f230::/48,2a03:2880:f231::/48,2a03:2880:f232::/48,2a03:2880:f233::/48,2a03:2880:f234::/48,2a03:2880:f235::/48,2a03:2880:f236::/48,2a03:2880:f237::/48,2a03:2880:f238::/48,2a03:2880:f23a::/48,2a03:2880:f23d::/48,2a03:2880:f23e::/48,2a03:2880:f23f::/48,2a03:2880:f240::/48,2a03:2880:f241::/48,2a03:2880:f242::/48,2a03:2880:f243::/48,2a03:2880:f244::/48,2a03:2880:f245::/48,2a03:2880:f247::/48,2a03:2880:f248::/48,2a03:2880:f24a::/48,2a03:2880:f24b::/48,2a03:2880:f24e::/48,2a03:2880:f24f::/48,2a03:2880:f250::/48,2a03:2880:f252::/48,2a03:2880:f253::/48,2a03:2880:f254::/48,2a03:2880:f256::/48,2a03:2880:f257::/48,2a03:2880:f258::/48,2a03:2880:f259::/48,2a03:2880:f25a::/48,2a03:2880:f25b::/48,2a03:2880:f25c::/48,2a03:2880:f25e::/48,2a03:2880:f260::/48,2a03:2880:f262::/48,2a03:2880:f263::/48,2a03:2880:f264::/48,2a03:2880:f265::/48,2a03:2880:f2ff::/48,2a03:2880:ff08::/48,2a03:2880:ff09::/48,2a03:2880:ff0a::/48,2a03:2880:ff0b::/48,2a03:2880:ff0c::/48,2a03:2881::/32,2a03:2881::/48,2a03:2881:1::/48,2a03:2881:2::/48,2a03:2881:3::/48,2a03:2881:4::/48,2a03:2881:5::/48,2a03:2881:6::/48,2a03:2881:7::/48,2a03:2881:8::/48,2a03:2881:9::/48,2a03:2881:a::/48,2a03:2881:4000::/48,2a03:2881:4001::/48,2a03:2881:4002::/48,2a03:2881:4003::/48,2a03:2881:4004::/48,2a03:2881:4006::/48",
		400)]
	public async Task BlacklistFacebook(string subnetAddressesString, int expectedCount)
	{
		var actualCount = 0;
		var subnetAddresstrings = subnetAddressesString.Split(',');

		foreach (var subnetAddressString in subnetAddresstrings)
		{
			var subnetAddress = Helpers.Networking.Models.AddressPrefix.Parse(subnetAddressString, null);

			await _sut.AddBlackholeAsync(subnetAddress);
			actualCount++;
		}

		Assert.Equal(expectedCount, actualCount);
	}

	[Theory(Skip = "adds 100 blackholes")]
	[InlineData("17.0.0.0/8", "17.0.0.0/9", "17.0.0.0/21", "17.0.64.0/18", "17.0.71.0/24", "17.0.99.0/24", "17.0.109.0/24", "17.0.117.0/24", "17.0.120.0/24", "17.0.122.0/24", "17.0.128.0/18", "17.0.141.0/24", "17.0.152.0/24", "17.0.176.0/20", "17.0.184.0/23", "17.0.224.0/24", "17.1.0.0/19", "17.1.64.0/18", "17.1.108.0/24", "17.1.128.0/18", "17.1.138.0/24", "17.1.184.0/24", "17.1.185.0/24", "17.1.193.0/24", "17.1.197.0/24", "17.7.64.0/24", "17.7.68.0/24", "17.7.72.0/24", "17.7.74.0/23", "17.8.64.0/23", "17.8.66.0/23", "17.8.68.0/22", "17.8.76.0/23", "17.8.80.0/23", "17.8.84.0/24", "17.8.85.0/24", "17.8.86.0/23", "17.8.90.0/24", "17.8.91.0/24", "17.8.130.0/24", "17.8.131.0/24", "17.8.136.0/24", "17.8.152.0/21", "17.8.192.0/20", "17.8.208.0/20", "17.8.224.0/20", "17.8.240.0/20", "17.13.64.0/20", "17.14.0.0/18", "17.14.64.0/19", "17.14.96.0/19", "17.15.16.0/22", "17.15.20.0/22", "17.15.48.0/23", "17.15.50.0/23", "17.15.52.0/22", "17.15.59.0/24", "17.15.61.0/24", "17.15.62.0/24", "17.15.63.0/24", "17.15.64.0/19", "17.15.96.0/20", "17.15.112.0/20", "17.15.128.0/20", "17.15.144.0/22", "17.15.148.0/22", "17.15.152.0/22", "17.15.156.0/23", "17.15.160.0/20", "17.15.176.0/20", "17.15.192.0/22", "17.15.196.0/22", "17.15.208.0/20", "17.15.224.0/20", "17.22.0.0/18", "17.22.64.0/19", "17.22.96.0/20", "17.22.112.0/21", "17.22.120.0/23", "17.23.0.0/22", "17.23.4.0/22", "17.23.8.0/22", "17.23.12.0/24", "17.23.14.0/23", "17.23.16.0/22", "17.23.20.0/23", "17.23.22.0/23", "17.23.24.0/22", "17.23.28.0/22", "17.23.32.0/23", "17.23.34.0/24", "17.23.36.0/24", "17.23.37.0/24", "17.23.38.0/24", "17.23.39.0/24", "17.23.40.0/24", "17.23.41.0/24", "17.23.42.0/24", "17.23.44.0/24", "17.23.48.0/20")]
	public async Task AddBlackHolesTests(params string[] strings)
	{
		var prefixes = strings.Select(p => AddressPrefix.Parse(p, CultureInfo.InvariantCulture)).ToArray();

		Assert.NotNull(prefixes);
		Assert.NotEmpty(prefixes);
		Assert.DoesNotContain(default, prefixes);

		await _sut.AddBlackholesAsync(prefixes);
	}

	[Theory(Skip = "expects exactly 400 blackholes")]
	[InlineData(400)]
	public async Task CountBlackholes(int expected)
	{
		Assert.Equal(
			expected,
			await _sut.GetBlackholesAsync().CountAsync());
	}

	[Fact]
	public Task RebootTests() => _sut.RebootAsync();
#pragma warning restore IDE0079, xUnit1004 // Remove unnecessary suppression; Test methods should not be skipped

	[Theory]
	[InlineData(OperationalStatus.Down)]
	[InlineData(OperationalStatus.Up)]
	public Task WifiTests(OperationalStatus status) => _sut.SetWifiStatusAsync(status);
	#endregion destructive tests
}
