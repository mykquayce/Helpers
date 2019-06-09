using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DockerSecretConfigurationExtensions
	{
		private static readonly string _path = Path.Combine(Path.DirectorySeparatorChar.ToString(), "run", "secrets");
		private static readonly DirectoryInfo _root = new DirectoryInfo(_path);

		/// <summary>
		/// Creates a config entry for each Docker secret with the name DockerSecrets:name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecrets(
			this IConfigurationBuilder configurationBuilder,
			bool optional = false,
			bool reloadOnChange = false)
		{
			if (configurationBuilder == default) throw new ArgumentNullException(nameof(configurationBuilder));

			foreach (var file in _root.EnumerateFiles())
			{
				configurationBuilder.AddDockerSecret(file.Name, optional, reloadOnChange);
			}

			return configurationBuilder;
		}

		/// <summary>
		/// Creates a config entry for the Docker secret with the name DockerSecrets:name
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="name"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecret(
			this IConfigurationBuilder configurationBuilder,
			string name,
			bool optional = false,
			bool reloadOnChange = false)
		{
			if (configurationBuilder == default) throw new ArgumentNullException(nameof(configurationBuilder));

			IFileProvider? fileProvider = _root.Exists && _root.EnumerateFiles().Any()
				? new PhysicalFileProvider(_root.FullName)
				: default;

			var configurationSource = new DockerSecretConfigurationSource
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
		/// Creates a config entry for each Docker secret with the name DockerSecrets:name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfiguration AddDockerSecrets(
			this IConfiguration configuration,
			bool optional = false,
			bool reloadOnChange = false)
		{
			if (configuration == default) throw new ArgumentNullException(nameof(configuration));

			var configurationBuilder = new ConfigurationBuilder();

			configurationBuilder.AddConfiguration(configuration);

			foreach (var file in _root.EnumerateFiles())
			{
				configurationBuilder.AddDockerSecret(file.Name, optional, reloadOnChange);
			}

			return configurationBuilder.Build();
		}

		/// <summary>
		/// Creates a config entry for the Docker secret with the name DockerSecrets:name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="name"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfiguration AddDockerSecret(
			this IConfiguration configuration,
			string name,
			bool optional = false,
			bool reloadOnChange = false)
		{
			return new ConfigurationBuilder()
				.AddConfiguration(configuration)
				.AddDockerSecret(name, optional, reloadOnChange)
				.Build();
		}
	}
}
