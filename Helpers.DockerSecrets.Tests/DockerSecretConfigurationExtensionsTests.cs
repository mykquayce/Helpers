using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Helpers.DockerSecrets.Tests
{
	public sealed class DockerSecretConfigurationExtensionsTests
	{
		[Theory]
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

		[Theory]
		[InlineData("hello", default, "world")]
		[InlineData("hello", "config:hello", "world")]
		public void AddDockerSecret(string fileName, string? configKey, string expected)
		{
			var configurationBuilder = new TestConfigurationBuilder();

			Assert.Empty(configurationBuilder.Sources);

			configurationBuilder
				.AddDockerSecret(fileName, configKey);

			Assert.NotEmpty(configurationBuilder.Sources);

			var configuration = configurationBuilder.Build();

			var actual = configuration.GetValue<string>(configKey ?? fileName);

			Assert.NotNull(actual);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(default)]
		[InlineData("")]
		[InlineData(" ")]
		[InlineData("\t")]
		[InlineData(" one")]
		[InlineData("one ")]
		[InlineData(" one ")]
		[InlineData("\tone")]
		[InlineData("\\")]
		[InlineData("/")]
		[InlineData(":")]
		[InlineData("*")]
		[InlineData("?")]
		[InlineData("\"")]
		[InlineData("<")]
		[InlineData(">")]
		[InlineData("|")]
		public void AddDockerSecret_FailValidation(string? fileName)
		{
			var configurationBuilder = new TestConfigurationBuilder();

			try
			{
				configurationBuilder
					.AddDockerSecret(fileName!);
			}
			catch (ArgumentException ex) when (ex.Source == "Dawn.Guard" && ex.ParamName == nameof(fileName))
			{ }
			catch (Exception ex)
			{
				Assert.True(false, ex.Message);
			}
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
