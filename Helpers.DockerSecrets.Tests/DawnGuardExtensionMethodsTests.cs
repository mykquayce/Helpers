using Dawn;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Helpers.DockerSecrets.Tests
{
	public class DawnGuardExtensionMethodsTests
	{
		[Theory]
		[InlineData("id_rsa.pub")]
		public void ValidConfigKey(string? configKey)
		{
			try
			{
				Guard.Argument(() => configKey).ValidConfigKey();
			}
			catch (ArgumentException ex)
			{
				Assert.True(false, ex.Message);
			}
		}
	}
}
