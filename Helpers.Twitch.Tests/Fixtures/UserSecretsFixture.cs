namespace Helpers.Twitch.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		private readonly Helpers.XUnitClassFixtures.UserSecretsFixture _fixture
			= new Helpers.XUnitClassFixtures.UserSecretsFixture();

		public string BearerToken => _fixture["Twitch:Client:BearerToken"];
		public string ClientId => _fixture["Twitch:Client:Id"];
		public string ClientSecret => _fixture["Twitch:Client:Secret"];
	}
}
