using Microsoft.Extensions.Options;
using System;

namespace Helpers.SSH.Tests.Fixtures
{
	public sealed class SSHServiceFixture : UserSecretsFixture, IDisposable
	{
		public SSHServiceFixture()
		{
			var config = base.Config;
			var options = Options.Create(config);
			SSHService = new Helpers.SSH.Services.Concrete.SSHService(options);
		}

		public Helpers.SSH.Services.ISSHService SSHService { get; }

		public void Dispose() => SSHService?.Dispose();
	}
}
