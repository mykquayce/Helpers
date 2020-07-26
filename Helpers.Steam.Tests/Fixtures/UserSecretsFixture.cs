using Helpers.MySql.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Helpers.Steam.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		private readonly Helpers.XUnitClassFixtures.UserSecretsFixture _fixture
			= new Helpers.XUnitClassFixtures.UserSecretsFixture();

		public string SteamKey => _fixture["SteamAPI:Key"];
		public IReadOnlyList<long> SteamIds => _fixture.Configuration.GetSection("SteamIds").Get<long[]>();
		public DbSettings DbSettings => _fixture.Configuration.GetSection(nameof(DbSettings)).Get<DbSettings>();
	}
}
