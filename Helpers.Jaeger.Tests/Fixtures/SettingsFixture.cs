using Helpers.Jaeger.Models;

namespace Helpers.Jaeger.Tests.Fixtures
{
	public class SettingsFixture
	{
		public Settings Settings { get; } = new("test-service", Settings.DefaultHost, Settings.DefaultPort, 1d);
	}
}
