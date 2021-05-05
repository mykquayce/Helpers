﻿using Dawn;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.Configuration
{
	public static class DockerSecretConfigurationExtensions
	{
		private static readonly string _path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
			? "C:\\ProgramData\\Docker\\secrets"
			: "/run/secrets";

		/// <summary>
		/// Creates a config entry for each Docker secret
		/// </summary>
		/// <param name="configurationBuilder"></param>
		/// <param name="optional"></param>
		/// <param name="reloadOnChange"></param>
		/// <param name="filenameCharsToSwapWithColons">case sensitive</param>
		/// <returns></returns>
		public static IConfigurationBuilder AddDockerSecrets(
			this IConfigurationBuilder configurationBuilder,
			bool optional = false,
			bool reloadOnChange = false,
			params char[] filenameCharsToSwapWithColons)
		{
			Guard.Argument(() => configurationBuilder).NotNull();

			IFileProvider fileProvider;
			try
			{
				fileProvider = new PhysicalFileProvider(_path, ExclusionFilters.Sensitive);
			}
			catch (DirectoryNotFoundException) when (optional)
			{
				return configurationBuilder;
			}
			catch { throw; }

			var files = fileProvider.GetDirectoryContents(string.Empty);

			foreach (var file in files)
			{
				var configKey = file.Name;

				foreach (var c in filenameCharsToSwapWithColons)
				{
					configKey = configKey.Replace(c, ':');
				}

				var source = new DockerSecretConfigurationSource(configKey)
				{
					FileProvider = fileProvider,
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
