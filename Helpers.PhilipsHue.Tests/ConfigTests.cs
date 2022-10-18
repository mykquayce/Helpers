using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.PhilipsHue.Tests;

public class ConfigTests
{
	[Theory]
	[InlineData("i35sdUz4iZI0XPWxbIdQKdp76t4cH8LOwUCtFcFJ", "https://discovery.meethue.com/")]
	public void Test1(string username, string discoveryEndPoint)
	{
		IOptions<Config>? actual;
		{
			var initialData = new Dictionary<string, string?>
			{
				[nameof(Config.DiscoveryEndPoint)] = discoveryEndPoint,
				[nameof(Config.Username)] = username,
			};

			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(initialData)
				.Build();

			using var serviceProvider = new ServiceCollection()
				.Configure<Config>(configuration)
				.BuildServiceProvider();

			actual = serviceProvider.GetService<IOptions<Config>>();
		}

		Assert.NotNull(actual);
		Assert.NotNull(actual.Value);
		Assert.Equal(username, actual.Value.Username);
		Assert.Equal(discoveryEndPoint, actual.Value.DiscoveryEndPoint.ToString());
	}
}
