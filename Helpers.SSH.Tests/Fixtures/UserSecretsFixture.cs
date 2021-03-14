using Microsoft.Extensions.Configuration;

namespace Helpers.SSH.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			var fixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

			Config = fixture.Configuration
				.GetSection("SSH")
				.Get<Helpers.SSH.Services.Concrete.SSHService.Config>();
		}

		public Helpers.SSH.Services.Concrete.SSHService.Config Config { get; }
	}
}
