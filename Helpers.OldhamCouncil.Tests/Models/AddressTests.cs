using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Helpers.OldhamCouncil.Tests.Models
{
	public class AddressTests
	{
		[Theory]
		[InlineData(@"[
	{
		""key"": ""422000069073"",
		""value"": ""21  OLDHAM DELIVERY OFFICE HAMILTON STREET OLDHAM OL1 1AA""
	}
]", "21", 422_000_069_073)]
		public void Deserialize(string json, string number, long id)
		{
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
			var addresses = JsonSerializer.Deserialize<IEnumerable<KeyValuePair<string, string>>>(json, options);

			Assert.NotNull(addresses);
			Assert.NotEmpty(addresses);
			Assert.Single(addresses);

			var dictionary = new Dictionary<string, long>(from kvp in addresses
														  let key = kvp.Value.Split(' ', count: 2, StringSplitOptions.RemoveEmptyEntries)[0]
														  let value = long.Parse(kvp.Key)
														  select new KeyValuePair<string, long>(key, value), StringComparer.InvariantCultureIgnoreCase);

			Assert.Contains(number, dictionary.Keys);
			Assert.Equal(id, dictionary[number]);
		}
	}
}
