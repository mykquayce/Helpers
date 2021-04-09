using Dawn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.Networking.Clients.Concrete
{
	public class WhoIsClient : TcpClient, IWhoIsClient
	{
		public WhoIsClient() : base("riswhois.ripe.net", 43)
		{ }

		public async IAsyncEnumerable<Models.SubnetAddress> GetIpsAsync(int asn)
		{
			Guard.Argument(() => asn).Positive();

			var message = $"-F -K -i {asn:D}\n";

			var lines = await SendAndReceiveAsync(message).ToListAsync();

			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line)) continue;
				if (line.StartsWith("% ERROR:", StringComparison.InvariantCultureIgnoreCase))
				{
					throw new WhoIsException(message, Hostname, Port, line[9..]);
				}
				if (line[0] == '%') continue;
				var parts = line.Split('\t');
				if (parts.Length < 2) continue;
				var address = Models.SubnetAddress.Parse(parts[1]);
				yield return address;
			}
		}

		public class WhoIsException : Exception
		{
			public WhoIsException(string message, string hostName, int port, string error)
				: base($@"Error sending ""{message}"" to {hostName}:{port:D}: {error}")
			{
				Data.Add(nameof(message), message);
				Data.Add(nameof(hostName), hostName);
				Data.Add(nameof(port), port);
				Data.Add(nameof(error), error);
			}
			public WhoIsException(string error)
				: base(error)
			{
				Data.Add(nameof(error), error);
			}
		}
	}
}
