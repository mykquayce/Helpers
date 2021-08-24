using Microsoft.Extensions.Configuration;

namespace Helpers.SSH.Tests.Fixtures;

public class UserSecretsFixture
{
	public UserSecretsFixture()
	{
		var fixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();

		Config = fixture.Configuration
			.GetSection("SSH")
			.Get<Config>()
			?? throw new KeyNotFoundException();
	}

	public Config Config { get; }
}
