using Dawn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Helpers.OldhamCouncil.Concrete
{
	public class Service : IService
	{
		private readonly IClient _client;
		private readonly static Regex _firstWordRegex = new(@"^(\d+|\w+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private readonly static ITypeDescriptionsLookupService<Models.BinTypes> _typeDescriptionsLookupService = new TypeDescriptionsLookupService<Models.BinTypes>();

		public Service(IClient client)
		{
			_client = Guard.Argument(client).NotNull().Value;
		}

		public async IAsyncEnumerable<KeyValuePair<string, long>> GetAddressesAsync(string postcode, CancellationToken? cancellationToken = default)
		{
			Guard.Argument(postcode).NotNull().NotEmpty().NotWhiteSpace();

			var kvps = _client.GetAddressesAsync(postcode, cancellationToken ?? CancellationToken.None);

			await foreach (var (id, address) in kvps)
			{
				var number = address.Split(' ', count: 2, StringSplitOptions.RemoveEmptyEntries)[0];
				yield return new(number, id);
			}
		}

		public async IAsyncEnumerable<KeyValuePair<DateTime, Models.BinTypes>> GetBinCollectionsAsync(long id, CancellationToken? cancellationToken = default)
		{
			Guard.Argument(id).Positive();

			var dictionary = new Dictionary<DateTime, Models.BinTypes>();

			var tables = _client.GetBinCollectionsAsync(id, cancellationToken ?? CancellationToken.None);

			await foreach (var table in tables)
			{
				var date = GetDateTime(table);
				var bin = GetBin(table);

				if (dictionary.TryAdd(date, bin))
				{
					continue;
				}

				dictionary[date] |= bin;
			}

			foreach (var kvp in dictionary)
			{
				yield return kvp;
			}
		}

		public async IAsyncEnumerable<KeyValuePair<DateTime, Models.BinTypes>> GetBinCollectionsAsync(string postcode, string? houseNumber, CancellationToken? cancellationToken = default)
		{
			var addresses = GetAddressesAsync(postcode, cancellationToken ?? CancellationToken.None);

			await foreach (var (no, id) in addresses)
			{
				if (houseNumber is null
					|| string.Equals(houseNumber, no, StringComparison.InvariantCultureIgnoreCase))
				{
					await foreach (var tuple in GetBinCollectionsAsync(id, cancellationToken ?? CancellationToken.None))
					{
						yield return tuple;
					}
				}
			}
		}

		public static Models.BinTypes GetBin(Models.Generated.tableType table)
		{
			var s = table.thead.tr.th[0].b;
			var match = _firstWordRegex.Match(s);
			var firstWord = match.Groups[1].Value;
			return _typeDescriptionsLookupService[firstWord];
		}

		public static DateTime GetDateTime(Models.Generated.tableType table)
			=> DateTime.ParseExact(table.tbody[0].td[1].Text[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
	}
}
