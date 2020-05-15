using Microsoft.Extensions.Options;
using System;

namespace Helpers.Steam.Tests.Fixtures
{
	public class SteamClientFixture : IDisposable
	{
		public SteamClientFixture()
		{
			var userSecretsFixture = new Fixtures.UserSecretsFixture();

			var settings = new Config.Settings { Key = userSecretsFixture.SteamKey, };

			var optionsSettings = Options.Create(settings);

			SteamClient = new Concrete.SteamClient(optionsSettings);
		}

		public ISteamClient SteamClient { get; }

		public void Dispose() => SteamClient?.Dispose();
	}
}
