using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Helpers.SSH.Tests;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2252:This API requires opting into preview features", Justification = "<Pending>")]
public class PublicKeyTests : IClassFixture<Fixtures.UserSecretsFixture>
{
	private readonly Config _config;

	public PublicKeyTests(Fixtures.UserSecretsFixture fixture)
	{
		_config = fixture.Config;
	}

	[Theory]
	[InlineData(10, 1_000)]
	public void ConnectionTests(int count, int pause)
	{
		var regex = new Regex(@"^\d+\s[\d\w:]+\s[\d\.]+\s.+?\s.+?$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

		Renci.SshNet.SshClient client;
		{
			var path = Concrete.Client.FixPath(_config.PathToPrivateKey!);
			using var keyFile = new Renci.SshNet.PrivateKeyFile(path);
			client = new Renci.SshNet.SshClient(_config.Host, _config.Port, _config.Username, keyFile);
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
			Assert.Matches(regex, output);

			Thread.Sleep(pause);
		}

		client.Disconnect();
		client.Dispose();
	}
}
