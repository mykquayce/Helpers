using Microsoft.Extensions.Configuration;
using System.IO;
using Xunit;

namespace Helpers.DockerSecrets.Tests
{
	public class PhysicalFileProviderTests
	{
		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public void MissingDirectoryTests(bool optional, bool success)
		{
			var configurationBuilder = new ConfigurationBuilder();

			try
			{
				configurationBuilder.AddDockerSecrets(optional: optional);
				Assert.True(success);
			}
			catch (DirectoryNotFoundException)
			{
				Assert.False(success);
			}
		}
	}
}
