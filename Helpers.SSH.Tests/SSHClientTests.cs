using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.SSH.Tests
{
	public class SSHClientTests : IClassFixture<Fixtures.UserSecretsFixture>
	{
		private readonly Helpers.SSH.Services.Concrete.SSHService.Config _config;

		public SSHClientTests(Fixtures.UserSecretsFixture fixture)
		{
			_config = fixture.Config;
			Assert.NotNull(_config.Host);
			Assert.InRange(_config.Port, 1, ushort.MaxValue);
			Assert.NotNull(_config.Username);
			Assert.NotNull(_config.Password);
			Assert.NotNull(_config.PathToPrivateKey);
			Assert.NotNull(_config.PathToPublicKey);
		}

		[Fact]
		public async Task Connect_ViaPassword()
		{
			using var client = new Clients.Concrete.SSHClient(_config.Host, _config.Port, _config.Username, _config.Password!);
			var output = await client.RunCommandAsync("echo");
			Assert.Contains(output, new[] { "\r", "\n", "\r\n", });
		}

		[Fact]
		public async Task Connect_ViaPrivateKey()
		{
			var path = Services.Concrete.SSHService.FixPath(_config.PathToPrivateKey!);
			var file = new FileInfo(path);
			using var client = new Clients.Concrete.SSHClient(_config.Host, _config.Port, _config.Username, file);
			var output = await client.RunCommandAsync("echo");
			Assert.Contains(output, new[] { "\r", "\n", "\r\n", });
		}
	}
}
