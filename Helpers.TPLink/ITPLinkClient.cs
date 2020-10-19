using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkClient : IDisposable
	{
		IAsyncEnumerable<Models.DeviceObject> GetDevicesAsync(string token);
		Task<string> LoginAsync(string userName, string password);
		Task<Models.ResponseDataObject.EmeterObject.GetRealtimeObject> GetRealtimeDataAsync(string token, string deviceId);
	}
}
