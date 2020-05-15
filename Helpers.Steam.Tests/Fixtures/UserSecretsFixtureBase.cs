using Microsoft.Extensions.Configuration;

namespace Helpers.Steam.Tests.Fixtures
{
	public abstract class UserSecretsFixtureBase
	{
		protected UserSecretsFixtureBase()
		{
			Configuration = new ConfigurationBuilder()
				.AddUserSecrets(this.GetType().Assembly)
				.Build();
		}

		protected IConfiguration Configuration { get; }
	}
}
