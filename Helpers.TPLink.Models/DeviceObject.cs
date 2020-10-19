namespace Helpers.TPLink.Models
{
	public class DeviceObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? deviceType { get; init; }
		public int? role { get; init; }
		public string? fwVer { get; init; }
		public string? appServerUrl { get; init; }
		public string? deviceRegion { get; init; }
		public string? deviceId { get; init; }
		public string? deviceName { get; init; }
		public string? deviceHwVer { get; init; }
		public string? alias { get; init; }
		public string? deviceMac { get; init; }
		public string? oemId { get; init; }
		public string? deviceModel { get; init; }
		public string? hwId { get; init; }
		public string? fwId { get; init; }
		public bool? isSameRegion { get; init; }
		public int? status { get; init; }
#pragma warning restore IDE1006 // Naming Styles
	}
}
