using Xunit;

namespace Helpers.SSH.Tests;

public class ClientTests : IClassFixture<Fixtures.UserSecretsFixture>
{
	private readonly Config _config;

	public ClientTests(Fixtures.UserSecretsFixture fixture)
	{
		_config = fixture.Config;
		Assert.NotNull(_config.Host);
		Assert.InRange(_config.Port, 1, ushort.MaxValue);
		Assert.NotNull(_config.Username);
		Assert.NotNull(_config.Password);
		Assert.NotNull(_config.PathToPrivateKey);
	}

	[Fact]
	public async Task Connect_ViaPassword()
	{
		var config = _config with { PathToPrivateKey = default, };
		using var client = new Concrete.Client(config);
		var output = await client.RunCommandAsync("echo");
		Assert.Contains(output, new[] { "\r", "\n", "\r\n", });
	}

	[Fact]
	public async Task Connect_ViaPrivateKey()
	{
		var config = _config with { Password = default, };
		using var client = new Concrete.Client(config);
		var output = await client.RunCommandAsync("echo");
		Assert.Contains(output, new[] { "\r", "\n", "\r\n", });
	}

	[Theory]
	[InlineData(@"C:\Users\bob\.ssh\id_rsa", @"C:\Users\bob\.ssh\id_rsa")]
	[InlineData("~/.ssh/id_rsa", @"C:\Users\bob\.ssh\id_rsa")]
	public void FixPath(string before, string expected)
	{
		var actual = Concrete.Client.FixPath(before);
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("arp -a")]
	public async Task RunCommandAsShell(string commandText)
	{
		ICollection<string> lines;
		{
			using var client = new Concrete.Client(_config);
			lines = await client.RunCommandAsShellAsync(commandText)
				.ToListAsync();
		}

		Assert.NotNull(lines);
		Assert.NotEmpty(lines);
		Assert.DoesNotContain(null, lines);
	}
}
