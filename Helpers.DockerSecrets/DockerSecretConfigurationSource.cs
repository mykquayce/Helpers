using Dawn;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationSource : FileConfigurationSource
	{
		private readonly string? _configKey;

		public DockerSecretConfigurationSource(string? configKey = default)
		{
			_configKey = Guard.Argument(() => configKey)
				.ValidConfigKey()
				.Value;
		}

		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			Guard.Argument(() => builder).NotNull();

			if (base.FileProvider is null)
			{
				base.FileProvider = builder.GetFileProvider();
			}

			return new DockerSecretConfigurationProvider(this, _configKey);
		}
	}
}
