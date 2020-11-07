using Xunit;

namespace Helpers.GlobalCache.Tests
{
	public class SerializationTests
	{
		[Theory]
		[InlineData("AMXB<-UUID=GlobalCache_000C1E059CAD><-SDKClass=Utility><-Make=GlobalCache><-Model=iTachIP2IR><-Revision=710-1005-05><-Pkg_Level=GCPK002><-Config-URL=http://192.168.1.113><-PCB_PN=025-0028-03><-Status=Ready>\r")]
		public void BeaconParse_AllPropertiesHaveValues(string s)
		{
			var beacon = Models.Beacon.Parse(s);

			Assert.NotNull(beacon.ConfigUrl);
			Assert.NotNull(beacon.Make);
			Assert.NotNull(beacon.Model);
			Assert.NotNull(beacon.PackageLevel);
			Assert.NotNull(beacon.PCB_PN);
			Assert.NotNull(beacon.Revision);
			Assert.NotNull(beacon.SDKClass);
			Assert.NotNull(beacon.Status);
			Assert.NotNull(beacon.Uuid);

			Assert.NotEmpty(beacon.ConfigUrl);
			Assert.NotEmpty(beacon.Make);
			Assert.NotEmpty(beacon.Model);
			Assert.NotEmpty(beacon.PackageLevel);
			Assert.NotEmpty(beacon.PCB_PN);
			Assert.NotEmpty(beacon.Revision);
			Assert.NotEmpty(beacon.SDKClass);
			Assert.NotEmpty(beacon.Status);
			Assert.NotEmpty(beacon.Uuid);
		}
	}
}
