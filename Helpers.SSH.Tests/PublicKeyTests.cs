using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;

namespace Helpers.SSH.Tests
{
	public class PublicKeyTests
	{
		[Theory]
		[InlineData(10, "192.168.1.10", 22, "root")]
		public void ConnectionTests(int count, string host, int port, string username)
		{
			Renci.SshNet.SshClient client;
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_rsa");
			{
				using var keyFile = new Renci.SshNet.PrivateKeyFile(path);
				client = new Renci.SshNet.SshClient(host, port, username, keyFile);
			}

			client.Connect();

			while (count-- > 0)
			{
				string output;
				{
					using var command = client.CreateCommand("cat /tmp/dhcp.leases", Encoding.UTF8);
					command.CommandTimeout = TimeSpan.FromSeconds(1);
					output = command.Execute();
				}

				Assert.NotNull(output);
				Assert.NotEmpty(output);
				var regex = new Regex(@"^\d+\s[\d\w:]+\s[\d\.]+\s.+?\s.+?$", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);
				Assert.Matches(regex, output);

				Thread.Sleep(millisecondsTimeout: 2_000);
			}

			client.Dispose();
		}
	}
}
