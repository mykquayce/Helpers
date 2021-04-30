using Microsoft.Extensions.Configuration;

namespace Helpers.TPLink.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			var fixture = new XUnitClassFixtures.UserSecretsFixture();

			Config = fixture.Configuration
				.GetSection("TPLink")
				.Get<Concrete.TPLinkWebClient.Config>();
		}

		public Concrete.TPLinkWebClient.Config Config { get; }
	}
}
