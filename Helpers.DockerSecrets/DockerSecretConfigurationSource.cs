using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationSource : FileConfigurationSource
	{
		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			if (builder is null) throw new ArgumentNullException(nameof(builder));

			if (base.FileProvider is null)
			{
				base.FileProvider = builder.GetFileProvider();
			}

			return new DockerSecretConfigurationProvider(this);
		}
	}
}
