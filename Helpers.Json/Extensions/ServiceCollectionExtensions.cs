using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
		{
			Converters =
			{
				new JsonStringEnumConverter(),
				new Helpers.Json.Converters.JsonBigIntegerConverter(),
				new Helpers.Json.Converters.JsonBoolConverter(),
				new Helpers.Json.Converters.JsonByteConverter(),
				new Helpers.Json.Converters.JsonDateTimeConverter(),
				new Helpers.Json.Converters.JsonCharConverter(),
				new Helpers.Json.Converters.JsonDoubleConverter(),
				new Helpers.Json.Converters.JsonFloatConverter(),
				new Helpers.Json.Converters.JsonGuidConverter(),
				new Helpers.Json.Converters.JsonIntConverter(),
				new Helpers.Json.Converters.JsonIPAddressConverter(),
				new Helpers.Json.Converters.JsonIPEndPointConverter(),
				new Helpers.Json.Converters.JsonLongConverter(),
				new Helpers.Json.Converters.JsonPhysicalAddressConverter(),
				new Helpers.Json.Converters.JsonSByteConverter(),
				new Helpers.Json.Converters.JsonShortConverter(),
				new Helpers.Json.Converters.JsonTimeSpanConverter(),
				new Helpers.Json.Converters.JsonUIntConverter(),
				new Helpers.Json.Converters.JsonULongConverter(),
				new Helpers.Json.Converters.JsonUriConverter(),
				new Helpers.Json.Converters.JsonUShortConverter(),
			},
		};

		public static IServiceCollection JsonConfig<TOptions>(this IServiceCollection services, IConfiguration configuration)
			where TOptions : class
		{
			var stringStringDictionary = configuration.Get<IReadOnlyDictionary<string, string>>();
			var json = JsonSerializer.Serialize(stringStringDictionary);
			var realDictionary = JsonSerializer.Deserialize<TOptions>(json, _jsonSerializerOptions);
			var options = Options.Options.Create(realDictionary);
			return services.AddSingleton(options);
		}
	}
}
