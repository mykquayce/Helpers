using Dawn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DockerSecretConfigurationExtensions
	{
		private static readonly string _path = Path.Combine(Path.DirectorySeparatorChar.ToString(), "run", "secrets");
		private static readonly DirectoryInfo _root = new DirectoryInfo(_path);

		/// <summary>
		/// Creates a config entry for each Docker secret with the name prefix:name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="prefix"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecrets(
			this IConfigurationBuilder configurationBuilder,
			string prefix,
			bool optional = false,
			bool reloadOnChange = false)
		{
			Guard.Argument(() => configurationBuilder).NotNull();
			Guard.Argument(() => prefix).NotNull().NotEmpty().NotWhiteSpace();

			foreach (var file in _root.EnumerateFiles())
			{
				configurationBuilder.AddDockerSecret(file.Name, prefix, optional, reloadOnChange);
			}

			return configurationBuilder;
		}

		/// <summary>
		/// Creates a config entry for the Docker secret with the name prefix:name
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="prefix"></param>
		/// <param name="name"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecret(
			this IConfigurationBuilder configurationBuilder,
			string prefix,
			string name,
			bool optional = false,
			bool reloadOnChange = false)
		{
			Guard.Argument(() => configurationBuilder).NotNull();
			Guard.Argument(() => prefix).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => name).NotNull().NotEmpty().NotWhiteSpace();

			IFileProvider? fileProvider = _root.Exists && _root.EnumerateFiles().Any()
				? new PhysicalFileProvider(_root.FullName)
				: default;

			var configurationSource = new DockerSecretConfigurationSource(prefix)
			{
				FileProvider = fileProvider,
				Path = name,
				Optional = optional,
				ReloadOnChange = reloadOnChange,
			};

			configurationBuilder.Add(configurationSource);

			return configurationBuilder;
		}


		/// <summary>
		/// Creates a config entry for each Docker secret with the name prefix:name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="prefix"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfiguration AddDockerSecrets(
			this IConfiguration configuration,
			string prefix,
			bool optional = false,
			bool reloadOnChange = false)
		{
			Guard.Argument(() => configuration).NotNull();
			Guard.Argument(() => prefix).NotNull().NotEmpty().NotWhiteSpace();

			var configurationBuilder = new ConfigurationBuilder();

			configurationBuilder.AddConfiguration(configuration);

			foreach (var file in _root.EnumerateFiles())
			{
				configurationBuilder.AddDockerSecret(file.Name, prefix, optional, reloadOnChange);
			}

			return configurationBuilder.Build();
		}

		/// <summary>
		/// Creates a config entry for the Docker secret with the name prefix:name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="prefix"></param>
		/// <param name="name"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfiguration AddDockerSecret(
			this IConfiguration configuration,
			string prefix,
			string name,
			bool optional = false,
			bool reloadOnChange = false)
		{
			Guard.Argument(() => configuration).NotNull();
			Guard.Argument(() => prefix).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => name).NotNull().NotEmpty().NotWhiteSpace();

			return new ConfigurationBuilder()
				.AddConfiguration(configuration)
				.AddDockerSecret(name, prefix, optional, reloadOnChange)
				.Build();
		}
	}
}
