using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Helpers.DockerSecrets.Tests
{
	public sealed class DockerSecretConfigurationExtensionsTests
	{
#pragma warning disable xUnit1004 // Test methods should not be skipped
		[Theory(Skip = "needs a file at /run/secrets/hello with the contents 'world'")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
		[InlineData("hello", "world")]
		public void AddDockerSecrets(string configKey, string expected)
		{
			var configurationBuilder = new TestConfigurationBuilder();

			Assert.Empty(configurationBuilder.Sources);

			configurationBuilder
				.AddDockerSecrets();

			Assert.NotEmpty(configurationBuilder.Sources);

			var configuration = configurationBuilder.Build();

			var actual = configuration.GetValue<string>(configKey);

			Assert.NotNull(actual);
			Assert.Equal(expected, actual);
		}

		public class TestConfigurationBuilder : IConfigurationBuilder
		{
			public IDictionary<string, object> Properties => throw new NotImplementedException();
			public IList<IConfigurationSource> Sources { get; } = new List<IConfigurationSource>();

			public IConfigurationBuilder Add(IConfigurationSource source)
			{
				Sources.Add(source);
				return this;
			}

			public IConfigurationRoot Build()
			{
				var providers = new List<IConfigurationProvider>();
				foreach (var source in Sources)
				{
					var provider = source.Build(this);
					providers.Add(provider);
				}
				return new ConfigurationRoot(providers);
			}
		}
	}
}
