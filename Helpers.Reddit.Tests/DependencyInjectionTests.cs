using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Reddit.Tests;

public class DependencyInjectionTests
{
	[Theory]
	[InlineData(@"{
  ""Blacklist"": [
    ""redd.it"",
    ""reddit.com"",
    ""redditmedia.com"",
    ""redditstatic.com""
  ]
}")]
	public async Task Test1(string json)
	{
		IService? sut;
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
					.AddSingleton(new XmlSerializerFactory())
					.AddHttpClient<IClient, Concrete.Client>(client => client.BaseAddress = new Uri("https://old.reddit.com/"))
					.ConfigurePrimaryHttpMessageHandler(() =>
					{
						return new HttpClientHandler { AllowAutoRedirect = false, };
					})
					.Services
					.Configure<List<string>>(configuration.GetSection("Blacklist"))
					.AddTransient<IService, Concrete.Service>()
					.BuildServiceProvider();
			}

			sut = serviceProvider.GetService<Helpers.Reddit.IService>();
		}

		Assert.NotNull(sut);

		var subredditName = await sut.GetRandomSubredditNameAsync();

		Assert.NotNull(subredditName);
		Assert.NotEmpty(subredditName);
	}
}
