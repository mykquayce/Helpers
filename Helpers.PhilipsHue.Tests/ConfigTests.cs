using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Helpers.PhilipsHue.Tests;

public class ConfigTests
{
	[Theory]
	[InlineData(null, "192.168.1.156", "i35sdUz4iZI0XPWxbIdQKdp76t4cH8LOwUCtFcFJ")]
	[InlineData("ecb5fa18e324", "192.168.1.156", "i35sdUz4iZI0XPWxbIdQKdp76t4cH8LOwUCtFcFJ")]
	public void Test1(string? physicalAddress, string? hostName, string username)
	{
		IOptions<Config>? actual;
		{
			var initialData = new Dictionary<string, string?>
			{
				["PhysicalAddress"] = physicalAddress,
				["HostName"] = hostName,
				["Username"] = username,
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
		Assert.Equal(physicalAddress, actual.Value.PhysicalAddress);
		Assert.Equal(hostName, actual.Value.HostName);
		Assert.Equal(username, actual.Value.Username);
		if (hostName is not null)
		{
			Assert.NotNull(actual.Value.BaseAddress);
			Assert.Equal($"http://{hostName}/", actual.Value.BaseAddress.OriginalString);
		}
		else
		{
			Assert.Null(actual.Value.BaseAddress);
		}
	}
}
