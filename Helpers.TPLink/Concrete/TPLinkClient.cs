using Dawn;
using Helpers.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.TPLink.Concrete
{
	public class TPLinkClient : Helpers.Web.WebClientBase, ITPLinkClient
	{
		public async IAsyncEnumerable<Models.DeviceObject> GetDevicesAsync(string token)
		{
			Guard.Argument(() => token).NotNull().NotEmpty().NotWhiteSpace().Matches("^6bdd39aa-[0-9A-Za-z]{23}$");

			var uri = new Uri("https://wap.tplinkcloud.com?token=" + token);

			var requestObject = new Models.GetDeviceListRequestObject();

			var requestBody = Serialize(requestObject);

			var (_, _, _, value) = await base.SendAsync<Models.GetDeviceListResponseObject>(HttpMethod.Post, uri, requestBody);

			foreach (var device in value!.result!.deviceList!)
			{
				yield return device;
			}
		}

		public async Task<Models.ResponseDataObject.EmeterObject.GetRealtimeObject> GetRealtimeDataAsync(string token, string deviceId)
		{
			Guard.Argument(() => token).NotNull().NotEmpty().NotWhiteSpace().Matches("^6bdd39aa-[0-9A-Za-z]{23}$");
			Guard.Argument(() => deviceId).NotNull().NotEmpty().NotWhiteSpace().Matches("^[0-9A-F]{40}$");

			var uri = new Uri("https://wap.tplinkcloud.com?token=" + token);

			var requestData = new Dictionary<string, IDictionary<string, object?>>
			{
				["system"] = new Dictionary<string, object?> { ["get_sysinfo"] = default, },
				["emeter"] = new Dictionary<string, object?> { ["get_realtime"] = default, },
			};

			var requestDataJson = Serialize(requestData);

			var requestObject = new Models.GetSysInfoRequestObject
			{
				@params = new Dictionary<string, string>()
				{
					["deviceId"] = deviceId,
					["requestData"] = requestDataJson,
				}
			};

			var requestBody = Serialize(requestObject, writeIndented: true);

			var (_, _, _, value) = await base.SendAsync<Models.GetSysInfoResponseObject>(HttpMethod.Post, uri, requestBody);

			var responseData = JsonSerializer.Deserialize<Models.ResponseDataObject>(value!.result.responseData!);

			return responseData?.emeter?.get_realtime
				?? throw new ArgumentNullException();
		}

		public async Task<string> LoginAsync(string userName, string password)
		{
			Guard.Argument(() => userName).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => password).NotNull().NotEmpty().NotWhiteSpace();

			var uri = new Uri("https://wap.tplinkcloud.com");

			var requestObject = new Models.LoginRequestObject
			{
				@params = new Models.LoginRequestObject.LoginParamsOjbect
				{
					cloudUserName = userName,
					cloudPassword = password,
					terminalUUID = Guid.NewGuid().ToString("d"),
				},
			};

			var requestBody = Serialize(requestObject);

			var (_, _, _, value) = await base.SendAsync<Models.LoginResponseObject>(HttpMethod.Post, uri, requestBody);

			return value?.result?.token ?? throw new ArgumentException();
		}

		private static string Serialize(object value, bool writeIndented = false)
		{
			var options = new JsonSerializerOptions { WriteIndented = writeIndented, };
			return JsonSerializer.Serialize(value, options).Replace("\\u0022", "\\\"");
		}
	}
}
