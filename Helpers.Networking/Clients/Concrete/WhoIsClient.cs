using Dawn;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Helpers.Networking.Clients.Concrete
{
	public class WhoIsClient : TcpClient, IWhoIsClient
	{
		private const RegexOptions _regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline;
		private readonly static IEnumerable<Regex> _ipRegices = new Regex[2]
		{
			new (@"^\d+\t(\d+\.\d+\.\d+\.\d+)\/(\d+)\t\d+\r?$", _regexOptions),
			new (@"^\d+\t((?:[0-9a-f]{1,4}:)+:)\/(\d+)\t\d+\r?$", _regexOptions),
		};

		public WhoIsClient() : base("riswhois.ripe.net", 43)
		{ }

		public async IAsyncEnumerable<Models.SubnetAddress> GetIpsAsync(int asn)
		{
			Guard.Argument(() => asn).Positive();

			var message = $"-F -K -i {asn:D}";

			var response = await SendAndReceiveAsync(message);
			var tuples = Parse(response);

			foreach (var tuple in tuples)
			{
				yield return tuple;
			}
		}

		public static IEnumerable<Models.SubnetAddress> Parse(string response)
		{
			Guard.Argument(() => response).NotNull();

			return _ipRegices.Select(r => r.Matches(response)).SelectMany(Parse);
		}

		public static IEnumerable<Models.SubnetAddress> Parse(IEnumerable<Match> matches)
		{
			Guard.Argument(() => matches).NotNull();

			foreach (var match in matches)
			{
				var ipString = match.Groups[1].Value;
				var subnetString = match.Groups[2].Value;

				var ip = IPAddress.Parse(ipString);
				var subnet = byte.Parse(subnetString);

				yield return new Models.SubnetAddress(ip, subnet);
			}
		}
	}
}
