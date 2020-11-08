using System.Collections.Generic;

namespace Helpers.Elgato.Models
{
	public record AccessoryInfoObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? productName { get; init; }
		public int? hardwareBoardType { get; init; }
		public int? firmwareBuildNumber { get; init; }
		public string? firmwareVersion { get; init; }
		public string? serialNumber { get; init; }
		public string? displayName { get; init; }
		public IList<string>? features { get; init; }
#pragma warning restore IDE1006 // Naming Styles
	}
}
