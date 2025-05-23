﻿using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Helpers.OpenWrt.Concrete;

public class Service : IService
{
	private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
	private static readonly Regex _blackholeRegex = new(@"\bblackhole (\d+\.\d+\.\d+\.\d+(?:\/\d+)?)\b", _regexOptions);

	private readonly IClient _client;

	public Service(IClient client)
	{
		ArgumentNullException.ThrowIfNull(client);
		_client = client;
	}

	public Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix prefix, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(prefix);
		return _client.ExecuteCommandAsync("ip route add blackhole " + prefix, cancellationToken);
	}

	public async Task AddBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> prefixes, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(prefixes);

		using var enumerator = prefixes.GetEnumerator();

		while (enumerator.MoveNext()
			&& !cancellationToken.IsCancellationRequested)
		{
			await AddBlackholeAsync(enumerator.Current, cancellationToken);
		}
	}

	public async IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var response = await _client.ExecuteCommandAsync("ip route show", cancellationToken);

		ArgumentException.ThrowIfNullOrWhiteSpace(response);

		var matches = _blackholeRegex.Matches(response);

		var enumerator = matches.GetEnumerator();

		while (enumerator.MoveNext()
			&& !cancellationToken.IsCancellationRequested)
		{
			var match = (Match)enumerator.Current;
			var s = match.Groups[1].Value;
			var prefix = Helpers.Networking.Models.AddressPrefix.Parse(s, provider: null);
			yield return prefix;
		}
	}

	public Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix blackhole, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(blackhole);
		return _client.ExecuteCommandAsync("ip route delete blackhole " + blackhole, cancellationToken);
	}
}
