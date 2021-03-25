using Dawn;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using System.IO;

namespace Microsoft.Extensions.Configuration
{
	public static class DockerSecretConfigurationExtensions
	{
		private static readonly string _path = Path.Combine(Path.DirectorySeparatorChar.ToString(), "run", "secrets");
		private static readonly IFileProvider _fileProvider = new PhysicalFileProvider(_path, ExclusionFilters.Sensitive);

		/// <summary>
		/// Creates a config entry for each Docker secret
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecrets(
			this IConfigurationBuilder configurationBuilder,
			bool optional = false,
			bool reloadOnChange = false)
		{
			Guard.Argument(() => configurationBuilder).NotNull();

			var files = _fileProvider.GetDirectoryContents(string.Empty);

			foreach (var file in files)
			{
				var configKey = file.Name;

				var source = new DockerSecretConfigurationSource(configKey)
				{
					FileProvider = _fileProvider,
					Optional = optional,
					Path = file.Name,
					ReloadOnChange = reloadOnChange,
				};

				configurationBuilder
					.Add(source);
			}

			return configurationBuilder;
		}
	}
}
