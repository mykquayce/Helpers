using Helpers.MySql.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Helpers.Steam.Tests.Fixtures
{
	public class UserSecretsFixture : UserSecretsFixtureBase
	{
		public string SteamKey => base.Configuration["SteamAPI:Key"];
		public IReadOnlyList<long> SteamIds => Configuration.GetSection("SteamIds").Get<long[]>();
		public DbSettings DbSettings => base.Configuration.GetSection(nameof(DbSettings)).Get<DbSettings>();
	}
}
