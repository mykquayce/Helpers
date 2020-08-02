using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Helpers.DockerSecrets.Tests
{
	public class DockerSecretConfigurationProviderTests
	{
		[Theory]
		[InlineData("")]
		[InlineData("hello world")]
		public void GetStreamContents(string message)
		{
			var bytes = Encoding.UTF8.GetBytes(message);
			using var stream = new MemoryStream(bytes);

			var actual = DockerSecretConfigurationProvider.GetStreamContents(stream);

			Assert.Equal(message, actual);
		}

		[Theory]
		[InlineData(default)]
		[InlineData("one")]
		[InlineData("one:two")]
		public void Ctor(string configKey)
		{
			var source = Mock.Of<FileConfigurationSource>();

			new DockerSecretConfigurationProvider(source, configKey);
		}

		[Theory]
		[InlineData("hello", default)]
		[InlineData("hello", "one")]
		[InlineData("hello", "one:two")]
		public void Load(string fileName, string? configKey)
		{
			// Arrange
			var path = Path.Combine("Data", fileName);
			var expected = File.ReadAllText(path);
			var key = configKey ?? fileName;
			var sut = new DockerSecretConfigurationProvider(Mock.Of<FileConfigurationSource>(), configKey);

			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				// Act
				sut.Load(stream);
			}

			// Assert
			Assert.NotNull(sut.Data);
			Assert.NotEmpty(sut.Data);
			Assert.Single(sut.Data);
			Assert.Contains(key, sut.Data.Keys);
			Assert.Equal(expected, sut.Data[key]);
		}

		[Theory]
		[InlineData("")]
		[InlineData("o ne")]
		[InlineData(" one")]
		[InlineData("one ")]
		[InlineData(" one ")]
		[InlineData("\t")]
		public void Ctor_FailValidation(string configKey)
		{
			try
			{
				new DockerSecretConfigurationProvider(Mock.Of<FileConfigurationSource>(), configKey);
				Assert.True(false, "Exception should've been thrown");
			}
			catch (ArgumentException ex) when (ex.Source == "Dawn.Guard" && ex.ParamName == nameof(configKey))
			{ }
			catch (Exception ex)
			{
				Assert.True(false, ex.Message);
			}
		}

		[Theory]
		[InlineData(" ", "")]
		[InlineData(" hello world", "hello world")]
		[InlineData("hello world ", "hello world")]
		[InlineData(" hello world ", "hello world")]
		public void Trim(string before, string expected)
		{
			var bytes = Encoding.UTF8.GetBytes(before);
			using var stream = new MemoryStream(bytes);

			var actual = DockerSecretConfigurationProvider.GetStreamContents(stream);

			Assert.Equal(expected, actual);
		}
	}
}
