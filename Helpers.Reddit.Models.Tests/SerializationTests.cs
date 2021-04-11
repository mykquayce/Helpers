using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Models.Tests
{
	public class SerializationTests
	{
		[Theory]
		[InlineData("worldnews.xml")]
		[InlineData("thread1.xml")]
		[InlineData("thread2.xml")]
		public async Task Deserialize_HasAllTheValues(string fileName)
		{
			var path = Path.Combine(".", "Data", fileName);

			await using var stream = File.OpenRead(path);

			var factory = new XmlSerializerFactory();

			var serializer = factory.CreateSerializer(typeof(Models.feed));

			var feed = serializer.Deserialize(stream) as Models.feed;

			Assert.NotNull(feed);
			AssertNotMalformedString(feed!.title);
			Assert.NotNull(feed.entry);
			Assert.NotEmpty(feed.entry);

			foreach (var entry in feed.entry)
			{
				Assert.NotNull(entry);
				Assert.NotNull(entry.author);
				AssertNotMalformedString(entry.author.name);
				Assert.NotNull(entry.content);
				AssertNotMalformedString(entry.content.Value);
				Assert.NotNull(entry.link);
				AssertNotMalformedString(entry.link.href);
				Assert.Null(entry.link.Value);
				Assert.NotEqual(default, entry.updated);
				Assert.InRange(entry.updated, DateTime.MinValue, DateTime.UtcNow);
				AssertNotMalformedString(entry.title);
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
}
