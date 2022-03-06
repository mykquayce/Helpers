using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Helpers.Reddit.Tests;

public class DeserializationTests
{
	[Theory]
	[InlineData(/*lang=json,strict*/ @"{""Denylist"":[""first"",""second""]}")]
	public void Test1(string json)
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var bytes = System.Text.Encoding.UTF8.GetBytes(json);
				using var stream = new MemoryStream(bytes);

				configuration = new ConfigurationBuilder()
					.AddJsonStream(stream)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.Configure<List<string>>(configuration.GetSection("Denylist"))
				.BuildServiceProvider();
		}

		var options = serviceProvider.GetService<IOptions<List<string>>>();

		Assert.NotNull(options);
		Assert.NotNull(options.Value);
		Assert.NotEmpty(options.Value);
		Assert.DoesNotContain(default, options.Value);
	}
}
