using System.Text.Json;
using Xunit;

namespace Helpers.Networking.Models.Tests;

public class WhoIsResponseTests
{
	[Theory]
	[InlineData(@"{""Prefix"":""163.114.128.0/20"",""ASN"":54115,""Description"":""FACEBOOK-CORP, US"",""NumRisPeers"":394}")]
	public void DeserializeSerialize(string before)
	{
		var o = JsonSerializer.Deserialize<WhoIsResponse>(before);
		var after = JsonSerializer.Serialize(o);
		Assert.Equal(before, after);
	}

	[Theory]
	[InlineData("163.114.128.0/20", 54_115, "FACEBOOK-CORP, US", 394)]
	public void SerializeDeserialize(string prefixString, int asn, string description, int numRisPeers)
	{
		var before = new WhoIsResponse(AddressPrefix.Parse(prefixString, null), asn, description, numRisPeers);
		var json = JsonSerializer.Serialize(before);
		var after = JsonSerializer.Deserialize<WhoIsResponse>(json);
		Assert.Equal(before, after);
	}

	[Theory]
	[InlineData(@"route:        163.114.128.0/20
origin:       AS54115
descr:        FACEBOOK-CORP, US
lastupd-frst: 2021-03-31 23:41Z  102.67.56.1@rrc00
lastupd-last: 2021-05-15 07:00Z  195.208.209.109@rrc13
seen-at:      rrc00,rrc01,rrc03,rrc04,rrc05,rrc06,rrc07,rrc10,rrc11,rrc12,rrc13,rrc14,rrc15,rrc16,rrc18,rrc19,rrc20,rrc21,rrc22,rrc23,rrc24,rrc25
num-rispeers: 394
source:       RISWHOIS", "163.114.128.0/20", 54_115, "FACEBOOK-CORP, US", 394)]
	public void ParseWhoIsResponseTests(string text, string expectedPrefix, int expectedASN, string expectedDescription, int expectedNumRisPeers)
	{
		var actual = ParseWhoIsResponse(text);

		Assert.NotNull(actual);
		Assert.Equal(expectedPrefix, actual.Prefix.ToString());
		Assert.Equal(expectedASN, actual.ASN);
		Assert.Equal(expectedDescription, actual.Description);
		Assert.Equal(expectedNumRisPeers, actual.NumRisPeers);
	}

	[Theory]
	[InlineData("-i 54115.txt")]
	public void ParseFile(string filename)
	{
		// Arrange
		var path = Path.Combine("Data", filename);
		var text = File.ReadAllText(path);

		// Act
		var models = ParseWhoIsResponses(text);

		// Assert
		Assert.NotNull(models);
		Assert.NotEmpty(models);

		foreach (var model in models)
		{
			Assert.NotNull(model);
			Assert.NotNull(model.Prefix);
			Assert.InRange(model.ASN, 1, int.MaxValue);
			Assert.NotNull(model.Description);
			Assert.InRange(model.NumRisPeers, 1, int.MaxValue);
		}
	}

	public static WhoIsResponse ParseWhoIsResponse(string text)
	{
		var lines = text.Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);

		var dictionary = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		foreach (var line in lines)
		{
			var kvp = line.Split(':', count: 2);
			var key = kvp[0].Trim();
			var value = kvp[1].Trim();
			dictionary.Add(key, value);
		}

		var prefix = AddressPrefix.Parse(dictionary.GetFirst("route", "route6"), null);
		var asn = int.Parse(dictionary["origin"][2..]);
		var description = dictionary["descr"];
		var numRisPeers = int.Parse(dictionary["num-rispeers"]);

		return new(prefix, asn, description, numRisPeers);
	}

	public static IEnumerable<WhoIsResponse> ParseWhoIsResponses(string text)
	{
		var entries = text.Split(new[] { "\r\r", "\n\n", "\r\n\r\n", }, StringSplitOptions.RemoveEmptyEntries);
		foreach (var entry in entries)
		{
			if (string.IsNullOrWhiteSpace(entry)) continue;
			if (entry.StartsWith('%')) continue;
			yield return ParseWhoIsResponse(entry);
		}
	}
}
