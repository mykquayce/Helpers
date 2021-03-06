﻿using Helpers.TPLink.Models;
using System.Threading.Tasks;

namespace Helpers.TPLink
{
	public interface ITPLinkUdpClient
	{
		Task<DiscoveryResponseObject.ResultObject> DiscoverAsync();
		Task<ResponseDataObject.EmeterObject.RealtimeObject> GetRealtimeAsync();
		Task<byte[]> SendAndReceiveBytesAsync(byte[] bytes);
		Task<string> SendAndReceiveMessageAsync(string message);
		Task<T> SendAndReceiveObjectAsync<T>(object requestObject);
	}
}
