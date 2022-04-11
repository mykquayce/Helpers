using System.Net.Sockets;
using System.Text;

namespace Helpers.GlobalCache.Tests;

public class EndToEndTests : IClassFixture<XUnitClassFixtures.UserSecretsFixture>
{
	private static readonly Encoding _encoding = Encoding.UTF8;
	private readonly System.Net.IPAddress _ipAddress;
	private readonly System.Net.EndPoint _endPoint;

	public EndToEndTests(XUnitClassFixtures.UserSecretsFixture fixture)
	{
		_ipAddress = System.Net.IPAddress.Parse(fixture["GlobalCache:IPAddress"]);
		_endPoint = new System.Net.IPEndPoint(_ipAddress, Config.Defaults.BroadcastPort);
	}

	[Theory]
	[InlineData("​sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	[InlineData("sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	[InlineData("sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,63,21,21,21,63,21,21,21,21,21,21,21,21,21,63,21,21,21,63,21,21,21,63,21,63,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	public async Task System_Net_Sockets_Socket(string message, string expected)
	{
		// Arrange
		var bytes = _encoding.GetBytes(message);

		// Act
		using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		await socket.ConnectAsync(_endPoint);
		await socket.SendAsync(bytes, SocketFlags.None);
		string actual;
		{
			var buffer = new byte[1_024];
			var bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
			var response = buffer[..bytesRead];
			actual = _encoding.GetString(response);
		}

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("​sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	[InlineData("sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	[InlineData("sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,63,21,21,21,63,21,21,21,21,21,21,21,21,21,63,21,21,21,63,21,21,21,63,21,63,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	public async Task Helpers_Networking_Clients_ISocketClient(string message, string expected)
	{
		// Arrange
		var bytes = _encoding.GetBytes(message);

		// Act
		Networking.Clients.ISocketClient socketClient = new Networking.Clients.Concrete.SocketClient(1_024, AddressFamily.InterNetwork, ProtocolType.Tcp, SocketType.Stream);
		await socketClient.ConnectAsync(_endPoint);
		await socketClient.SendAsync(bytes);
		var responseBytes = await socketClient.ReceiveAsync();
		var actual = _encoding.GetString(responseBytes);

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("​sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	[InlineData("sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	[InlineData("sendir,1:2,14,37996,1,1,343,168,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,63,21,21,21,63,21,21,21,63,21,21,21,21,21,21,21,21,21,63,21,21,21,63,21,21,21,63,21,63,21,63,21,1512,343,84,21,4858\r", "completeir,1:2,14\r")]
	public async Task Helpers_GlobalCache_IService(string message, string expected)
	{
		// Act
		using IService sut = new Helpers.GlobalCache.Concrete.Service(Config.Defaults);
		await sut.ConnectAsync(_ipAddress);
		var response = await sut.SendAndReceiveAsync(message);

		// Assert
		Assert.Equal(expected, response);
	}
}
