using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.GlobalCache.Tests;

[Collection(nameof(CollectionDefinition.NonParallelCollectionDefinitionClass))]
public class BeaconTests
{
	[Theory]
	[InlineData("AMXB<-UUID=GlobalCache_000C1E059CAD><-SDKClass=Utility><-Make=GlobalCache><-Model=iTachIP2IR><-Revision=710-1005-05><-Pkg_Level=GCPK002><-Config-URL=http://192.168.1.113><-PCB_PN=025-0028-03><-Status=Ready>\r")]
	public void Parse(string s)
	{
		static void DoAssert(string? s)
		{
			Assert.NotNull(s);
			Assert.NotEmpty(s);
			Assert.NotEqual(' ', s![0]);
			Assert.NotEqual(' ', s[^1]);
		}

		var beacon = Models.Beacon.Parse(s);

		DoAssert(beacon.ConfigUrl);
		DoAssert(beacon.Make);
		DoAssert(beacon.Model);
		DoAssert(beacon.PackageLevel);
		DoAssert(beacon.PCB_PN);
		DoAssert(beacon.Revision);
		DoAssert(beacon.SDKClass);
		DoAssert(beacon.Status);
		DoAssert(beacon.Uuid);
	}

	[Theory]
	[InlineData(
		"AMXB<-UUID=GlobalCache_000C1E059CAD><-SDKClass=Utility><-Make=GlobalCache><-Model=iTachIP2IR><-Revision=710-1005-05><-Pkg_Level=GCPK002><-Config-URL=http://192.168.1.113><-PCB_PN=025-0028-03><-Status=Ready>\r",
		"000c1e059cad",
		"192.168.1.113"
		)]
	public void GetAddresses(string s, string expectedPhysicalAddressString, string expectedIPAddressString)
	{
		var beacon = Models.Beacon.Parse(s);

		var expectedPhysicalAddress = PhysicalAddress.Parse(expectedPhysicalAddressString);
		var expectedIPAddress = IPAddress.Parse(expectedIPAddressString);

		Assert.Equal(expectedPhysicalAddress, beacon.PhysicalAddress);
		Assert.Equal(expectedIPAddress, beacon.IPAddress);
	}
}
