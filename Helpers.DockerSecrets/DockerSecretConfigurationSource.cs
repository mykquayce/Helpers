using Dawn;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationSource : FileConfigurationSource
	{
		private readonly string _prefix;

		public DockerSecretConfigurationSource(string prefix)
		{
			_prefix = Guard.Argument(() => prefix).NotNull().NotEmpty().NotWhiteSpace().Value;
		}

		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			Guard.Argument(() => builder).NotNull();

			if (base.FileProvider is null)
			{
				base.FileProvider = builder.GetFileProvider();
			}

			return new DockerSecretConfigurationProvider(_prefix, this);
		}
	}
}
