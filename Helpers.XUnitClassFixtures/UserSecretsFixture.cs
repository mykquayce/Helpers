using Microsoft.Extensions.Configuration;

namespace Helpers.XUnitClassFixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			Configuration = new ConfigurationBuilder()
				.AddUserSecrets(this.GetType().Assembly)
				.Build();
		}

		public string this[string key] => Configuration[key];
		public IConfiguration Configuration { get; }
		public T GetSection<T>(string section) => Configuration.GetSection(section).Get<T>();
	}
}
