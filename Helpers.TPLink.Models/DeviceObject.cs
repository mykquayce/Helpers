namespace Helpers.TPLink.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record DeviceObject(
		string alias,
		string appServerUrl,
		string deviceHwVer,
		string deviceId,
		string deviceMac,
		string deviceModel,
		string deviceName,
		string deviceRegion,
		string deviceType,
		string fwId,
		string fwVer,
		string hwId,
		bool isSameRegion,
		string oemId,
		int role,
		int status);
#pragma warning restore IDE1006 // Naming Styles
}
