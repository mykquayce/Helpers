using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Helpers.Json.Tests
{
	public class DependencyInjectionWithoutConverters
	{
		[Theory]
		[InlineData(@"{""AmpSmartPlug"":""003192e1a474"",""IRBlaster"":""000c1e059cad""}")]
		public void Test1(string json)
		{
			var options = new JsonSerializerOptions
			{
				Converters =
				{
					new Converters.JsonPhysicalAddressConverter(),
				},
			};

			IConfiguration configuration;
			{
				var bytes = Encoding.UTF8.GetBytes(json);
				using var stream = new MemoryStream(bytes);

				configuration = new ConfigurationBuilder()
					.AddJsonStream(stream)
					.Build();
			}

			var dictionary = configuration.Get<IReadOnlyDictionary<string, object>>();

			var jsonAgain = JsonSerializer.Serialize(dictionary);

			var t = JsonSerializer.Deserialize<IDictionary<Devices, PhysicalAddress>>(jsonAgain, options);
			var o = Options.Create(t);

			var provider = new ServiceCollection()
				.AddSingleton(o)
				.BuildServiceProvider();

			var optionsAgain = provider.GetService<IOptions<IDictionary<Devices, PhysicalAddress>>>();

			Assert.NotNull(optionsAgain);
		}

		[Theory]
		//[InlineData(@"{""AmpSmartPlug"":""003192e1a474"",""IRBlaster"":""000c1e059cad""}")]
		[InlineData(@"{""AmpSmartPlug"":1,""IRBlaster"":2}")]
		public void Dictionary_StringInt(string json)
		{
			IConfiguration configuration;
			{
				var bytes = Encoding.UTF8.GetBytes(json);
				using var stream = new MemoryStream(bytes);

				configuration = new ConfigurationBuilder()
					.AddJsonStream(stream)
					.Build();
			}

			var provider = new ServiceCollection()
				.Inject<IReadOnlyDictionary<string, int>>(configuration)
				.BuildServiceProvider();

			var options = provider.GetService<IOptions<IReadOnlyDictionary<string, int>>>();

			Assert.NotNull(options);
			Assert.NotNull(options!.Value);
			Assert.NotEmpty(options.Value);
		}

		[Flags]
		public enum Devices : byte
		{
			None = 0,
			AmpSmartPlug = 1,
			IRBlaster = 2,
		}

		public class DevicesObject : Dictionary<Devices, PhysicalAddress> { }
	}

	public static class Extensions
	{
		public static IConfigurationBuilder AddJsonString(this IConfigurationBuilder builder, string json)
		{
			throw new NotImplementedException();
		}

		public static IServiceCollection Inject<TOptions>(this IServiceCollection services, IConfiguration configuration)
			where TOptions : class
		{
			var stringStringDictionary = configuration.Get<IReadOnlyDictionary<string, string>>();

			var stringObjectDictionary = (
				from kvp in stringStringDictionary
				let key = kvp.Key
				let value = int.Parse(kvp.Value)
				select new KeyValuePair<string, int>(key, value)
			).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			var json = JsonSerializer.Serialize(stringObjectDictionary);
			var @object = JsonSerializer.Deserialize<TOptions>(json);
			var injectable = Options.Create(@object);
			services.AddSingleton(injectable);
			return services;
		}
	}
}
