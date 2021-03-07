using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Helpers.SSH.Tests.Fixtures
{
	public sealed class SSHServiceFixture : IDisposable
	{
		public SSHServiceFixture()
		{
			var fixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			var config = fixture.Configuration
				.GetSection("SSH")
				.Get<Helpers.SSH.Services.Concrete.SSHService.Config>();

			var options = Options.Create(config);

			SSHService = new Helpers.SSH.Services.Concrete.SSHService(options);

		}

		public Helpers.SSH.Services.ISSHService SSHService { get; }

		public void Dispose() => SSHService?.Dispose();
	}
}
