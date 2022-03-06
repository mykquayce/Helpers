using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Models.Tests;

public class SerializationTests
{
	[Theory]
	[InlineData(".", "Data", "worldnews.xml")]
	[InlineData(".", "Data", "thread1.xml")]
	[InlineData(".", "Data", "thread2.xml")]
	public async Task Deserialize_HasAllTheValues(params string[] paths)
	{
		Generated.feedType? feed;
		{
			var path = Path.Combine(paths);
			await using var stream = File.OpenRead(path);
			var serializer = new XmlSerializer(typeof(Generated.feedType));
			feed = serializer.Deserialize(stream) as Generated.feedType;
		}

		Assert.NotNull(feed);
		Assert.NotNull(feed.entry);
		Assert.NotEmpty(feed.entry);

		foreach (var entry in feed.entry)
		{
			Assert.NotNull(entry);
			Assert.NotNull(entry.content);
			AssertNotMalformedString(entry.content.Value);
			Assert.NotNull(entry.link);
			AssertNotMalformedString(entry.link.href);
			Assert.Null(entry.link.Value);
		}
	}

	private static void AssertNotMalformedString(string? s)
	{
		Assert.NotNull(s);
		Assert.NotEmpty(s);
		Assert.DoesNotMatch(@"^\s+$", s);
		Assert.DoesNotMatch(@"^\s", s);
		Assert.DoesNotMatch(@"\s$", s);
	}
}
