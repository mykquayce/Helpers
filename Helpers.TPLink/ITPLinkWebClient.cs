﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkWebClient : IDisposable
	{
		IAsyncEnumerable<Models.DeviceObject> GetDevicesAsync(string token);
		Task<string> LoginAsync(string userName, string password);
		Task<Models.ResponseDataObject.EmeterObject.RealtimeObject> GetRealtimeDataAsync(string token, string deviceId);
	}
}
