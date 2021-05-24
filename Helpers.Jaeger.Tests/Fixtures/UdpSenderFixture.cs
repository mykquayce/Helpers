using Jaeger.Senders;
using Jaeger.Senders.Thrift;
using Jaeger.Thrift.Senders.Internal;

namespace Helpers.Jaeger.Tests.Fixtures
{
	public class UdpSenderFixture
	{
		public UdpSenderFixture()
		{
			var settings = new SettingsFixture().Settings;
			Sender = new UdpSender(settings.Host, settings.Port, maxPacketSize: ThriftUdpClientTransport.MaxPacketSize);
		}

		public ISender Sender { get; }
	}
}
