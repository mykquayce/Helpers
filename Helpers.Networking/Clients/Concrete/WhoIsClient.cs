﻿using System.Net;
using System.Runtime.CompilerServices;

namespace Helpers.Networking.Clients.Concrete;

public class WhoIsClient : TcpClient, IWhoIsClient
{
	private readonly static char[] _lineBreaks = { '\r', '\n', };
	private readonly static string[] _doubleLineBreaks = { "\r\r", "\n\n", "\r\n\r\n", };

	public WhoIsClient() : base("riswhois.ripe.net", 43, "\n")
	{ }

	public async IAsyncEnumerable<Models.AddressPrefix> GetIpsAsync(int asn, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(asn);

		var message = $"-F -K -i {asn:D}\n";

		var lines = SendAndReceiveAsync(message, cancellationToken);

		await foreach (var line in lines)
		{
			if (string.IsNullOrWhiteSpace(line)) continue;
			if (line.StartsWith("% ERROR:", StringComparison.OrdinalIgnoreCase))
			{
				throw new Exceptions.WhoIsException(message, Hostname, Port, line[9..]);
			}
			if (line[0] == '%') continue;
			var parts = line.Split('\t');
			if (parts.Length < 2) continue;
			yield return Models.AddressPrefix.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
		}
	}

	public async IAsyncEnumerable<Models.WhoIsResponse> GetWhoIsDetailsAsync(IPAddress ipAddress, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(ipAddress);

		var message = ipAddress.ToString() + "\n";

		var lines = await SendAndReceiveAsync(message, cancellationToken).ToListAsync(cancellationToken);

		var error = lines.FirstOrDefault(s => s.StartsWith("% ERROR:", StringComparison.OrdinalIgnoreCase));
		if (error is not null) throw new Exceptions.WhoIsException(message, Hostname, Port, error[9..]);

		var text = string.Join('\r', lines);

		foreach (var response in GetWhoIsResponses(text))
		{
			yield return response;
		}
	}

	public static IEnumerable<Models.WhoIsResponse> GetWhoIsResponses(string text)
	{
		var entries = text.Split(_doubleLineBreaks, StringSplitOptions.RemoveEmptyEntries);

		foreach (var entry in entries)
		{
			if (string.IsNullOrWhiteSpace(entry)) continue;
			if (entry.StartsWith('%')) continue;

			yield return GetWhoIsResponse(entry);
		}
	}

	public static Models.WhoIsResponse GetWhoIsResponse(string text)
	{
		var lines = text.Split(_lineBreaks, StringSplitOptions.RemoveEmptyEntries);

		var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		foreach (var line in lines)
		{
			var kvp = line.Split(':', count: 2);
			var key = kvp[0].Trim();
			var value = kvp[1].Trim();
			dictionary.Add(key, value);
		}

		var prefix = Models.AddressPrefix.Parse(dictionary.GetFirst("route", "route6"), System.Globalization.CultureInfo.InvariantCulture);
		var asn = int.Parse(dictionary["origin"][2..]);
		var description = dictionary["descr"];
		var numRisPeers = int.Parse(dictionary["num-rispeers"]);

		return new(prefix, asn, description, numRisPeers);
	}
}
