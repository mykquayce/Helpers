using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Xml.Serialization;

namespace Helpers.Timing.Tests;

public class DeserializationTest
{
	private static readonly XmlSerializerFactory _xmlSerializerFactory = new();

	[Theory]
	[InlineData(@"{""Unit"":""Day"",""Count"":2}", Units.Day, 2)]
	[InlineData(@"{""Unit"":""Hour"",""Count"":0.5}", Units.Hour, .5)]
	[InlineData(@"{""Unit"":""Minute"",""Count"":30}", Units.Minute, 30)]
	public void JsonDeserializationTests(string json, Units unit, double count)
	{
		IInterval? actual = JsonSerializer.Deserialize<Interval>(json);

		Assert.NotNull(actual);
		Assert.Equal(unit, actual.Unit);
		Assert.Equal(count, actual.Count);
	}

	[Theory]
	[InlineData(@"<Interval><Unit>Day</Unit><Count>2</Count></Interval>", Units.Day, 2)]
	[InlineData(@"<Interval><Unit>Hour</Unit><Count>.5</Count></Interval>", Units.Hour, .5)]
	[InlineData(@"<Interval><Unit>Hour</Unit><Count>0.5</Count></Interval>", Units.Hour, .5)]
	[InlineData(@"<Interval><Unit>Minute</Unit><Count>30</Count></Interval>", Units.Minute, 30)]
	public void XmlDeserializationTests(string xml, Units unit, double count)
	{
		IInterval? actual;
		{
			var serializer = _xmlSerializerFactory.CreateSerializer(typeof(Interval));

			var bytes = System.Text.Encoding.UTF8.GetBytes(xml);
			using var stream = new MemoryStream(bytes);
			actual = serializer.Deserialize(stream) as Interval;
		}

		Assert.NotNull(actual);
		Assert.Equal(unit, actual.Unit);
		Assert.Equal(count, actual.Count);
	}

	[Theory]
	[InlineData(Units.Day, 2)]
	[InlineData(Units.Hour, .5)]
	[InlineData(Units.Minute, 30)]
	public void DataBindingTests(Units unit, double count)
	{
		IConfiguration configuration;
		{
			var initialData = new Dictionary<string, string?>
			{
				["Unit"] = unit.ToString("F"),
				["Count"] = count.ToString(),
			};

			configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();
		}

		IInterval actual = new Interval();
		configuration.Bind(actual);

		Assert.Equal(unit, actual.Unit);
		Assert.Equal(count, actual.Count);
	}

	[Theory]
	[InlineData(Units.Day, 2)]
	[InlineData(Units.Hour, .5)]
	[InlineData(Units.Minute, 30)]
	public void DependencyInjectionTests(Units unit, double count)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var initialData = new Dictionary<string, string?>
				{
					["Unit"] = unit.ToString("F"),
					["Count"] = count.ToString(),
				};

				configuration = new ConfigurationBuilder()
					.AddInMemoryCollection(initialData)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.Configure<Interval>(configuration)
				.BuildServiceProvider();
		}

		var options = serviceProvider.GetRequiredService<IOptions<Interval>>();

		Assert.NotNull(options);

		IInterval actual = options.Value;

		Assert.NotNull(actual);
		Assert.Equal(unit, actual.Unit);
		Assert.Equal(count, actual.Count);
	}
}
