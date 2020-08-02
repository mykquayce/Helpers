using Dawn;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DockerSecretConfigurationExtensions
	{
		private static readonly IFileProvider _fileProvider;

		private static readonly IEnumerable<string> _paths = new string[3]
		{
			Path.Combine(Path.DirectorySeparatorChar.ToString(), "run", "secrets"),
			new DirectoryInfo("Data").FullName,
			new DirectoryInfo(".").FullName,
		};

		static DockerSecretConfigurationExtensions()
		{
			foreach (var path in _paths)
			{
				if (Directory.Exists(path))
				{
					_fileProvider = new PhysicalFileProvider(path, ExclusionFilters.Sensitive);
					return;
				}
			}

			throw new Exception("Unable to locate a secrets directory");
		}

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

			foreach (var file in _fileProvider.GetDirectoryContents(string.Empty))
			{
				var configKey = file.Name;

				configurationBuilder
					.AddDockerSecret(file.Name, configKey, optional, reloadOnChange);
			}

			return configurationBuilder;
		}

		/// <summary>
		/// Creates a config entry for each Docker secret with the name: configKey ?? fileName
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="fileName">The name of the file in /run/secrets</param>
		/// <param name="configKey">The key of the config entry</param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecret(
			this IConfigurationBuilder configurationBuilder,
			string fileName,
			string? configKey = default,
			bool optional = false,
			bool reloadOnChange = false)
		{
			var invalidFileNameChars = Path.GetInvalidFileNameChars();

			Guard.Argument(() => configurationBuilder).NotNull();
			Guard.Argument(() => fileName).ValidFileName();
			Guard.Argument(() => configKey).ValidConfigKey();

			var file = _fileProvider.GetFileInfo(fileName);

			if (file.Exists)
			{
				var source = new DockerSecretConfigurationSource(configKey ?? fileName)
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


		/// <summary>
		/// Creates a config entry for each Docker secret with the name: configKey ?? fileName
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
			Guard.Argument(() => configuration).NotNull();

			return new ConfigurationBuilder()
				.AddConfiguration(configuration)
				.AddDockerSecrets(optional, reloadOnChange)
				.Build();
		}

		/// <summary>
		/// Creates a config entry for each Docker secret with the name: configKey ?? fileName
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="fileName">The name of the file in /run/secrets</param>
		/// <param name="configKey">The key of the config entry</param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <returns></returns>
		public static IConfiguration AddDockerSecret(
			this IConfiguration configuration,
			string fileName,
			string? configKey = default,
			bool optional = false,
			bool reloadOnChange = false)
		{
			Guard.Argument(() => configuration).NotNull();

			return new ConfigurationBuilder()
				.AddConfiguration(configuration)
				.AddDockerSecret(fileName, configKey, optional, reloadOnChange)
				.Build();
		}
	}
}
