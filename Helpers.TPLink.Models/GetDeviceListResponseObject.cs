using System.Collections.Generic;

namespace Helpers.TPLink.Models
{
	public class GetDeviceListResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public Models.Enums.ErrorCode? error_code { get; init; }
		public ResultObject? result { get; init; }

		public class ResultObject
		{
			public IList<DeviceObject>? deviceList { get; init; }

		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
