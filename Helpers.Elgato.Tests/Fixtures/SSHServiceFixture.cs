namespace Helpers.Elgato.Tests.Fixtures
{
	public class SSHServiceFixture
	{
		public SSHServiceFixture()
		{
			var fixture = new ConfigFixture();
			SSHService = new Helpers.SSH.Services.Concrete.SSHService(fixture.SSHConfig);
		}

		public Helpers.SSH.Services.ISSHService SSHService { get; }
	}
}
