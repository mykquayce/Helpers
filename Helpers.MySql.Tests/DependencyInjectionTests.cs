using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Xunit;

namespace Helpers.MySql.Tests;

public class DependencyInjectionTests : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
{
	private readonly string _username, _password;

	public DependencyInjectionTests(Helpers.XUnitClassFixtures.UserSecretsFixture fixture)
	{
		_username = fixture["MySql:Username"];
		_password = fixture["MySql:Password"];
	}

	[Fact]
	public async Task Test1()
	{
		IServiceProvider serviceProvider;
		{
			IConfiguration configuration;
			{
				var jsonString = $@"{{
  ""Server"": ""localhost"",
  ""Port"": 3306,
  ""UserId"": ""{_username}"",
  ""Password"": ""{_password}"",
  ""Secure"": true
}}";
				var bytes = Encoding.UTF8.GetBytes(jsonString);
				await using var stream = new MemoryStream(bytes);

				configuration = new ConfigurationBuilder()
					.AddJsonStream(stream)
					.Build();
			}

			serviceProvider = new ServiceCollection()
				.AddDbConnection(configuration)
				.AddTransient<ITestRepository, TestRepository>()
				.BuildServiceProvider();
		}

		var sut = serviceProvider.GetRequiredService<ITestRepository>();

		var now = DateTime.UtcNow;
		var actual = await sut.GetDateTimeAsync();
		Assert.InRange(actual, now.AddSeconds(-2), now.AddSeconds(2));

		(serviceProvider as ServiceProvider)?.Dispose();
	}
}
