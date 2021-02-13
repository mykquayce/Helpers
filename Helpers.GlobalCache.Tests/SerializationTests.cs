using Xunit;

namespace Helpers.GlobalCache.Tests
{
	[Collection("Non-Parallel Collection")]
	public class SerializationTests
	{
		[Theory]
		[InlineData("AMXB<-UUID=GlobalCache_000C1E059CAD><-SDKClass=Utility><-Make=GlobalCache><-Model=iTachIP2IR><-Revision=710-1005-05><-Pkg_Level=GCPK002><-Config-URL=http://192.168.1.113><-PCB_PN=025-0028-03><-Status=Ready>\r")]
		public void BeaconParse_AllPropertiesHaveValues(string s)
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
	}
}
