using Dawn;
using Helpers.Reddit.Models.Generated;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Models.Tests;

public class DawnGuardTests(XmlSerializerFactory xmlSerializerFactory) : IClassFixture<XmlSerializerFactory>
{
	private readonly XmlSerializer _xmlSerializer = xmlSerializerFactory.CreateSerializer(typeof(feedType));

	[Theory]
	[InlineData("worldnews.xml")]
	[InlineData("thread1.xml")]
	[InlineData("thread2.xml")]
	public async Task ThreadIdTests(string fileName)
	{
		feedType feed;
		{
			var path = Path.Combine(".", "Data", fileName);
			await using var stream = new FileStream(path: path, FileMode.Open);
			feed = (feedType)_xmlSerializer.Deserialize(stream)!;
		}

		foreach (var entry in feed.entry)
		{
			switch (entry.id[1])
			{
				case '1': // comment
				case '3': // link
					Guard.Argument(entry.id).IsId();
					break;
				default:
					Assert.True(false);
					break;
			}
		}
	}
}
