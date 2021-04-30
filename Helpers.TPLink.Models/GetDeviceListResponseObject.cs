using System.Collections.Generic;

namespace Helpers.TPLink.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record GetDeviceListResponseObject(Enums.ErrorCode error_code, GetDeviceListResponseObject.ResultObject result)
		: IResponse
	{
		public record ResultObject(IList<DeviceObject> deviceList);
	}
#pragma warning restore IDE1006 // Naming Styles
}
