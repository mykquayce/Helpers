using Dawn;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Helpers.Steam.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			var configuration = new ConfigurationBuilder()
				.AddUserSecrets(this.GetType().Assembly)
				.Build();

			var key = configuration["SteamAPI:Key"];
			var steamIds = configuration.GetSection("SteamAPI:SteamIds").Get<long[]>();

			Key = Guard.Argument(() => key).NotNull().NotEmpty().NotWhiteSpace().Value;
			SteamIds = Guard.Argument(() => steamIds).NotNull().NotEmpty().DoesNotContainNull().Value;
		}

		public string Key { get; }
		public IReadOnlyList<long> SteamIds { get; }
	}
}
