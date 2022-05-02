using Dawn;
using System.Text.RegularExpressions;

namespace Helpers.OpenWrt.Concrete;

public class Service : IService
{
	private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
	private static readonly Regex _blackholeRegex = new(@"\bblackhole (\d+\.\d+\.\d+\.\d+(?:\/\d+)?)\b", _regexOptions);

	private readonly IClient _client;

	public Service(IClient client)
	{
		_client = Guard.Argument(client).NotNull().Value;
	}

	public Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix prefix, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(prefix).NotNull();
		return _client.ExecuteCommandAsync("ip route add blackhole " + prefix, cancellationToken);
	}

	public async Task AddBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> prefixes, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(prefixes).NotNull();

		using var enumerator = prefixes.GetEnumerator();

		while (enumerator.MoveNext()
			&& cancellationToken?.IsCancellationRequested != true)
		{
			await AddBlackholeAsync(enumerator.Current, cancellationToken);
		}
	}

	public async IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync(CancellationToken? cancellationToken = default)
	{
		var response = await _client.ExecuteCommandAsync("ip route show", cancellationToken);

		Guard.Argument(response).NotNull().NotEmpty().NotWhiteSpace();

		var matches = _blackholeRegex.Matches(response);

		var enumerator = matches.GetEnumerator();

		while (enumerator.MoveNext()
			&& cancellationToken?.IsCancellationRequested != true)
		{
			var match = (Match)enumerator.Current;
			var s = match.Groups[1].Value;
			var prefix = Helpers.Networking.Models.AddressPrefix.Parse(s);
			yield return prefix;
		}
	}

	public Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix blackhole, CancellationToken? cancellationToken = default)
	{
		Guard.Argument(blackhole).NotNull();
		return _client.ExecuteCommandAsync("ip route delete blackhole " + blackhole, cancellationToken);
	}
}
