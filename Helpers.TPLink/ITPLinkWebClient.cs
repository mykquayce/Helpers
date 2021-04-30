using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkWebClient : IDisposable
	{
		IAsyncEnumerable<Models.DeviceObject> GetDevicesAsync();
		Task<string> LoginAsync();
		Task<Models.ResponseDataObject.EmeterObject.RealtimeObject> GetRealtimeDataAsync(string deviceId);
	}
}
