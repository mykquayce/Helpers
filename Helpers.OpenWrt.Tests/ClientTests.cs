using System.Text.Json;
using Xunit;

namespace Helpers.OpenWrt.Tests;

public sealed class ClientTests : IClassFixture<Fixtures.ClientFixture>
{
	private readonly IClient _sut;

	public ClientTests(Fixtures.ClientFixture fixture)
	{
		_sut = fixture.Client;
	}

	[Theory]
	[InlineData("echo -n 'hello world'", "^hello world$")]
	[InlineData("ip route show", @"(\d+\.\d+\.\d+\.\d+)")]
	public async Task ExecuteCommand(string command, string expected)
	{
		var actual = await _sut.ExecuteCommandAsync(command);
		Assert.Matches(expected, actual);
	}

	[Theory]
	[InlineData("/tmp/dhcp.leases")]
	public async Task GetFileContent(string path)
	{
		var s = await _sut.GetFileContentsAsync(path);

		Assert.NotNull(s);
	}

	[Theory]
	[InlineData("/tmp/dhcp.leases")]
	public async Task GetFileDetails(string path)
	{
		Models.StatResultObject details = await _sut.GetFileDetailsAsync(path);

		Assert.NotNull(details);
		Assert.InRange(details.size, 1, int.MaxValue);
		Assert.Matches(@"^[r-][w-][x-][r-][w-][x-][r-][w-][x-]$", details.modestr);
	}

	[Fact]
	public async Task EndToEnd()
	{
		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://192.168.1.10"), };

		async Task<T> send<T>(string requestUri, Models.RequestObject requestObject)
		{
			var requestJson = JsonSerializer.Serialize(requestObject);
			using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
			{
				Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json"),
			};

			using var responseMessage = await httpClient!.SendAsync(requestMessage);
			var responseJson = await responseMessage.Content.ReadAsStringAsync();
			var responseObject = JsonSerializer.Deserialize<Models.ResponseObject<T>>(responseJson);
			if (responseObject!.error is not null)
			{
				throw new Exception($" '{responseObject.error.message} ({responseObject.error.code})' '{responseObject.error.data}'");
			}
			return responseObject!.result;
		}

		var loginRequestObject = new Models.LoginRequestObject(
			Username: "root",
			Password: "5d2d7995d1b846695bba6d54291557bf56d084dab71ddb7df3777e6259dc6655");

		var token = await send<string>("/cgi-bin/luci/rpc/auth", loginRequestObject);

		var requestObject = new Models.RequestObject(1, "net.arptable");

		var result = await send<string?>("/cgi-bin/luci/rpc/sys?auth=" + token, requestObject);

		Assert.NotNull(result);

		Convert.FromBase64String(result);

		/*{"id":1,"result":{"cfg050f15":{".name":"cfg050f15",".type":"device","name":"wan","macaddr":"ea:9f:80:19:f0:10",".anonymous":true,".index":4},"lan":{".name":"lan",".anonymous":false,"dns":["192.168.1.60"],"device":"br-lan",".index":3,"gateway":"192.168.1.2","ipaddr":"192.168.1.10","ip6assign":"60",".type":"interface","netmask":"255.255.255.0","proto":"static"},"globals":{".name":"globals",".type":"globals","ula_prefix":"fd5a:a1ba:fc99::/48",".anonymous":false,".index":1},"cfg030f15":{".name":"cfg030f15",".type":"device","name":"br-lan","ports":["lan1","lan2","lan3","lan4"],"type":"bridge",".anonymous":true,".index":2},"wan":{".name":"wan",".type":"interface","device":"wan","proto":"dhcp",".anonymous":false,".index":5},"loopback":{".name":"loopback",".type":"interface","ipaddr":"127.0.0.1","netmask":"255.0.0.0","device":"lo","proto":"static",".anonymous":false,".index":0},"wan6":{".name":"wan6",".type":"interface","device":"wan","proto":"dhcpv6",".anonymous":false,".index":6}},"error":null}*/
	}

	[Theory]
	[InlineData(@"{""id"":1,""result"":{"".name"":""default_radio0"","".anonymous"":false,""ssid"":""SKY3317E"",""encryption"":""psk2"",""device"":""radio0"",""key"":""3c50f3123fd3793e"","".type"":""wifi-iface"",""macaddr"":""e8:9f:80:19:f0:12"",""network"":""lan"",""mode"":""ap""},""error"":null}")]
	public void Deserialize(string json)
	{
		var o = JsonSerializer.Deserialize<Models.ResponseObject<Models.WifiDeviceResponseObject>>(json);
	}
}
/*{"id":1,"result":{"dev":16,"type":"reg","modedec":644,"rdev":0,"nlink":1,"atime":1651398612,"blocks":8,"modestr":"rw-r--r--","ino":2183,"mtime":1651586495,"gid":0,"blksize":4096,"ctime":1651586495,"uid":0,"size":722},"error":null}*/

public class ResponseException : Exception
{
	public ResponseException(Models.ResponseObject.ErrorObject errorObject)
		: this(errorObject.message, errorObject.data, errorObject.code)
	{ }

	public ResponseException(string message, string data, int code)
		: base(message: $"{message} ({code}): '{data}'")
	{
		Data.Add(nameof(message), message);
		Data.Add(nameof(data), data);
		Data.Add(nameof(code), code);
	}
}
