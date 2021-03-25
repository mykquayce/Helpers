using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace Helpers.DockerSecrets.Tests
{
	public class DockerSecretConfigurationSourceTests
	{
		[Theory]
		[InlineData(default)]
		[InlineData("one")]
		[InlineData("one:two")]
		public void Ctor(string? prefix)
		{
			try
			{
#pragma warning disable CA1806 // Do not ignore method results
				new DockerSecretConfigurationSource(prefix);
#pragma warning restore CA1806 // Do not ignore method results
			}
			catch (Exception ex)
			{
				Assert.True(false, ex.Message);
			}
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
#pragma warning disable CA1806 // Do not ignore method results
				new DockerSecretConfigurationSource(configKey);
#pragma warning restore CA1806 // Do not ignore method results
				Assert.True(false, "Exception should've been thrown");
			}
			catch (ArgumentException ex) when (ex.Source == "Dawn.Guard" && ex.ParamName == nameof(configKey))
			{ }
			catch (Exception ex)
			{
				Assert.True(false, ex.Message);
			}
		}
	}
}
