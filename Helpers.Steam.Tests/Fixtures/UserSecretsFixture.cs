using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Helpers.Steam.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		private readonly Helpers.XUnitClassFixtures.UserSecretsFixture _fixture = new();

		public string SteamKey => _fixture["SteamAPI:Key"];
		public IReadOnlyList<long> SteamIds => _fixture.Configuration.GetSection("SteamIds").Get<long[]>();
		public Helpers.MySql.Config DbSettings => _fixture.Configuration.GetSection(nameof(DbSettings)).Get<MySql.Config>();
	}
}
