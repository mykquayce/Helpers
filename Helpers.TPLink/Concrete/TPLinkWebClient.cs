using Dawn;
using Helpers.Web.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.TPLink.Concrete
{
	public class TPLinkWebClient : Helpers.Web.WebClientBase, ITPLinkWebClient
	{
		public record Config(string UserName, string Password)
		{
			public Config() : this("admin", "admin") { }
		}

		private readonly string _userName, _password;
		private string? _token;

		public TPLinkWebClient(IOptions<Config> options) : this(options.Value) { }
		public TPLinkWebClient(Config config)
		{
			Guard.Argument(() => config).NotNull();
			_userName = Guard.Argument(() => config.UserName).NotNull().NotEmpty().NotWhiteSpace().Value;
			_password = Guard.Argument(() => config.Password).NotNull().NotEmpty().NotWhiteSpace().Value;
		}

		private string Token => _token ??= LoginAsync().GetAwaiter().GetResult();

		public async IAsyncEnumerable<Models.DeviceObject> GetDevicesAsync()
		{
			var uri = new Uri("https://wap.tplinkcloud.com?token=" + Token);

			var requestObject = new Models.GetDeviceListRequestObject();

			var requestBody = Serialize(requestObject);

			var (_, _, _, value) = await base.SendAsync<Models.GetDeviceListResponseObject>(HttpMethod.Post, uri, requestBody);

			foreach (var device in value!.result!.deviceList!)
			{
				yield return device;
			}
		}

		public async Task<Models.ResponseDataObject.EmeterObject.RealtimeObject> GetRealtimeDataAsync(string deviceId)
		{
			Guard.Argument(() => deviceId).NotNull().NotEmpty().NotWhiteSpace().Matches("^[0-9A-F]{40}$");

			var uri = new Uri("https://wap.tplinkcloud.com?token=" + Token);

			var requestData = new Dictionary<string, IDictionary<string, object?>>(StringComparer.InvariantCultureIgnoreCase)
			{
				["system"] = new Dictionary<string, object?> { ["get_sysinfo"] = default, },
				["emeter"] = new Dictionary<string, object?> { ["get_realtime"] = default, },
			};

			var requestDataJson = Serialize(requestData);

			var requestObject = new Models.GetSysInfoRequestObject
			{
				@params = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
				{
					["deviceId"] = deviceId,
					["requestData"] = requestDataJson,
				},
			};

			var requestBody = Serialize(requestObject, writeIndented: true);

			var (_, _, _, value) = await base.SendAsync<Models.GetSysInfoResponseObject>(HttpMethod.Post, uri, requestBody);

			var responseData = JsonSerializer.Deserialize<Models.ResponseDataObject>(value!.result!.responseData!);

			return responseData?.emeter?.get_realtime
				?? throw new ArgumentNullException();
		}

		public async Task<string> LoginAsync()
		{
			var uri = new Uri("https://wap.tplinkcloud.com");

			var requestObject = new Models.LoginRequestObject(_userName, _password);

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
