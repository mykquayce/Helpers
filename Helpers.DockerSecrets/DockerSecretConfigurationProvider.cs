using Dawn;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
	public class DockerSecretConfigurationProvider : FileConfigurationProvider
	{
		private readonly string _prefix;

		public DockerSecretConfigurationProvider(string prefix, FileConfigurationSource source) : base(source)
		{
			_prefix = Guard.Argument(() => prefix).NotNull().NotEmpty().NotWhiteSpace().Value;
		}

		public override void Load(Stream stream)
		{
			Guard.Argument(() => stream).NotNull().Require(s => s is FileStream).Require(s => ((FileStream)s).CanRead);

			var fileStream = (FileStream)stream;
			var name = Path.GetFileName(fileStream.Name);

			Data.Add(
				key: string.Concat(_prefix, ":", name),
				value: GetStreamContents(stream));
		}

		private static string GetStreamContents(Stream stream)
		{
			using var reader = new StreamReader(stream);

			return reader.ReadToEnd();
		}
	}
}
