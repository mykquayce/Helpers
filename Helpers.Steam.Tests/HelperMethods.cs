using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Steam.Tests
{
	public static class HelperMethods
	{
		public async static Task<T> ReadAndDeserializeFileAsync<T>(string fileName)
		{
			Assert.NotNull(fileName);

			var path = Path.Combine(Environment.CurrentDirectory, "Data", fileName);

			Assert.True(File.Exists(path), path + " does not exist");

			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
			return await JsonSerializer.DeserializeAsync<T>(stream)
				?? throw new Exception();
		}
	}
}
