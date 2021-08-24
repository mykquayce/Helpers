namespace Helpers.SSH.Tests.Fixtures;

public sealed class LibraryFixture : UserSecretsFixture, IDisposable
{
	public LibraryFixture()
	{
		var config = base.Config;

		if (config.Password is not null)
		{
			Library = new(config.Host, config.Port, config.Username, config.Password);
		}
		else if (config.PathToPrivateKey is not null)
		{
			var path = Concrete.Client.FixPath(config.PathToPrivateKey);
			var privateKey = new Renci.SshNet.PrivateKeyFile(path);
			Library = new(config.Host, config.Port, config.Username, privateKey);
		}
		else
		{
			Library = new(config.Host, config.Port, config.Username);
		}

		Library.Connect();
	}

	public Renci.SshNet.SshClient Library { get; }

	public void Dispose()
	{
		if (Library.IsConnected) Library.Disconnect();
		Library.Dispose();
	}
}
