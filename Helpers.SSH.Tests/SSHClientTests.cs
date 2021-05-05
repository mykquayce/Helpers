using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.SSH.Tests
{
	public class SSHClientTests
	{
		[Theory]
		[InlineData("192.168.1.10", 22, "root", "***REMOVED***")]
		public async Task ConnectViaPasswordTest(string host, ushort port, string username, string password)
		{
			using var client = new Clients.Concrete.SSHClient(host, port, username, password);
			var output = await client.RunCommandAsync("echo");
			Assert.Equal("\n", output);
		}

		[Theory]
		[InlineData("192.168.1.10", 22, "root", "~/.ssh/id_rsa")]
		public async Task ConnectViaPrivateKey(string host, ushort port, string username, string pathToPrivateKey)
		{
			var path = Services.Concrete.SSHService.FixPath(pathToPrivateKey);
			var file = new FileInfo(path);
			using var client = new Clients.Concrete.SSHClient(host, port, username, file);
			var output = await client.RunCommandAsync("echo");
			Assert.Equal("\n", output);
		}

		[Theory]
		[InlineData(@"C:\Users\bob\.ssh\id_rsa", @"C:\Users\bob\.ssh\id_rsa")]
		[InlineData("~/.ssh/id_rsa", @"C:\Users\bob\.ssh\id_rsa")]
		public void FixPath(string before, string expected)
		{
			var actual = Services.Concrete.SSHService.FixPath(before);
			Assert.Equal(expected, actual);
		}
	}
}
