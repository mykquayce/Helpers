using System;

namespace Helpers.SSH.Tests.Fixtures
{
	public sealed class LibraryFixture : UserSecretsFixture, IDisposable
	{
		public LibraryFixture()
		{
			var config = base.Config;

			var host = config.Host;
			var port = config.Port;
			var username = config.Username;
			var password = config.Password;

			Library = new Renci.SshNet.SshClient(host, port, username, password);

			Library.Connect();
		}

		public Renci.SshNet.SshClient Library { get; }

		public void Dispose()
		{
			if (Library.IsConnected) Library.Disconnect();
			Library.Dispose();
		}
	}
}
