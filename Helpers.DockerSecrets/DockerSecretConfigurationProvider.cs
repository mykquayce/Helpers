using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationProvider : FileConfigurationProvider
	{
		public DockerSecretConfigurationProvider(FileConfigurationSource source) : base(source)
		{ }

		public override void Load(Stream stream)
		{
			if (stream == default) throw new ArgumentNullException(nameof(stream));

			if (!(stream is FileStream fileStream))
			{
				throw new ArgumentOutOfRangeException(nameof(stream), stream.GetType(), "Expecting a " + nameof(FileStream))
				{ Data = { [nameof(stream)] = stream.GetType() }, };
			}

			if (!fileStream.CanRead)
			{
				throw new ArgumentException("Unreadable stream", nameof(fileStream))
				{
					Data = { [nameof(fileStream.Name)] = fileStream.Name, }
				};
			}

			var key = "DockerSecrets:" + Path.GetFileName(fileStream.Name);

			string value;

			using (var reader = new StreamReader(stream))
			{
				value = reader.ReadToEnd();
			}

			Data.Add(key, value);
		}
	}
}
