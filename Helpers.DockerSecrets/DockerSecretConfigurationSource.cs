using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationSource : FileConfigurationSource
	{
		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			if (builder == default) throw new ArgumentNullException(nameof(builder));

			if (base.FileProvider == default)
			{
				base.FileProvider = builder.GetFileProvider();
			}

			return new DockerSecretConfigurationProvider(this);
		}
	}
}
