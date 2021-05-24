using Jaeger.Reporters;

namespace Helpers.Jaeger.Tests.Fixtures
{
	public class ReporterFixture
	{
		public ReporterFixture()
		{
			var sender = new UdpSenderFixture().Sender;

			Reporter = new RemoteReporter.Builder()
				.WithSender(sender)
				.Build();
		}

		public IReporter Reporter { get; }
	}
}
