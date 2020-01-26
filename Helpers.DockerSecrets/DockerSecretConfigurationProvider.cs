using Dawn;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationProvider : FileConfigurationProvider
	{
		private readonly string? _configKey;

		public DockerSecretConfigurationProvider(FileConfigurationSource source, string? configKey = default)
			: base(source)
		{
			_configKey = Guard.Argument(() => configKey)
				.ValidConfigKey()
				.Value;
		}

		public new IDictionary<string, string> Data => base.Data;

		public override void Load(Stream stream)
		{
			Guard.Argument(() => stream)
				.NotNull()
				.Require(s => s is FileStream)
				.Require(s => s.CanRead, _ => nameof(stream) + " is readonly")
				.Require(s => s.Length > 0, _ => nameof(stream) + " is empty");

			var key = _configKey ?? Path.GetFileName(((FileStream)stream).Name);
			var value = GetStreamContents(stream);

			Data.Add(key, value);
		}

		internal static string GetStreamContents(Stream stream)
		{
			using var reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
	}
}
