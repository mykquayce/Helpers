using System;
using System.Collections.Generic;

namespace Helpers.Elgato.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record AccessoryInfoObject(
		string productName,
		int hardwareBoardType,
		int firmwareBuildNumber,
		Version firmwareVersion,
		string serialNumber,
		string displayName,
		IList<string> features);
#pragma warning restore IDE1006 // Naming Styles
}
