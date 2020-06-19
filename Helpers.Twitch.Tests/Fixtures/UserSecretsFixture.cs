using Microsoft.Extensions.Configuration;
using System;

namespace Helpers.Twitch.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		private const string _bearerTokenKey = "Twitch:Client:BearerToken";
		private const string _clientIdKey = "Twitch:Client:Id";
		private const string _clientSecretKey = "Twitch:Client:Secret";

		public UserSecretsFixture()
		{
			var config = new ConfigurationBuilder()
				.AddUserSecrets<UnitTest1>(optional: false, reloadOnChange: true)
				.Build();

			BearerToken = config[_bearerTokenKey] ?? throw new ArgumentNullException(nameof(_bearerTokenKey));
			ClientId = config[_clientIdKey] ?? throw new ArgumentNullException(nameof(_clientIdKey));
			ClientSecret = config[_clientSecretKey] ?? throw new ArgumentNullException(nameof(_clientSecretKey));
		}

		public string BearerToken { get; }
		public string ClientId { get; }
		public string ClientSecret { get; set; }
	}
}
