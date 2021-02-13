using Microsoft.Extensions.Configuration;
using System.Net.NetworkInformation;

namespace Helpers.Elgato.Tests.Fixtures
{
	public class UserSecretsFixture
	{
		public UserSecretsFixture()
		{
			var @base = new Helpers.XUnitClassFixtures.UserSecretsFixture();
			Configuration = @base.Configuration;
		}

		public IConfiguration Configuration { get; }

		public PhysicalAddress PhysicalAddress => PhysicalAddress.Parse(Configuration["Elgato:EndPoint:PhysicalAddress"]);
		public int Port => int.Parse(Configuration["Elgato:EndPoint:Port"]);
	}
}
