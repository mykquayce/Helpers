using System.Xml.Serialization;
using Xunit;

namespace Helpers.Common.Tests;

public class ProjectTests
{
	[Theory(Skip = "changes .csproj files")]
	[InlineData("Helpers.Cineworld.Models.csproj")]
	[InlineData("Helpers.Common.csproj")]
	[InlineData("Helpers.DawnGuard.csproj")]
	[InlineData("Helpers.DockerSecrets.csproj")]
	[InlineData("Helpers.Json.csproj")]
	[InlineData("Helpers.MySql.csproj")]
	[InlineData("Helpers.OldhamCouncil.csproj")]
	[InlineData("Helpers.Phasmophobia.csproj")]
	[InlineData("Helpers.Reddit.Models.csproj")]
	[InlineData("Helpers.Steam.Models.csproj")]
	[InlineData("Helpers.Telegram.Models.csproj")]
	[InlineData("Helpers.Twitch.csproj")]
	public void Deserialze(string projectFileName)
	{
		var root = GetSolutionRoot();
		var projectFile = root.GetFile(projectFileName);

		var project = Deserialize<Models.Generated.ProjectType>(projectFile);
		if (project.IsTestProject()) return;
		project.PropertyGroup.TargetFramework = "netstandard2.0";
		var xml = Serialize(project);
		projectFile.Save(xml);
	}

	private static DirectoryInfo GetSolutionRoot()
	{
		var directory = new DirectoryInfo(Environment.CurrentDirectory);

		while (!directory.EnumerateFiles("*.sln", SearchOption.TopDirectoryOnly).Any())
		{
			directory = directory.Parent ?? throw new Exception();
		}

		return directory;
	}

	private static readonly XmlSerializerFactory _xmlSerializerFactory = new();

	private static T Deserialize<T>(FileInfo file)
		where T : class
	{
		var serializer = _xmlSerializerFactory.CreateSerializer(typeof(T));
		using var stream = file.OpenRead();
		return serializer.Deserialize(stream) as T ?? throw new Exception("deserialization failed");
	}

	private static string Serialize(object o)
	{
		var serializer = _xmlSerializerFactory.CreateSerializer(o.GetType());
		using var stream = new MemoryStream();
		serializer.Serialize(stream, o);
		stream.Position = 0;
		using var reader = new StreamReader(stream);
		return reader.ReadToEnd();
	}
}

public static class Extensions
{
	public static FileInfo GetFile(this DirectoryInfo root, string fileName)
	{
		return root.EnumerateFiles(fileName, SearchOption.AllDirectories).FirstOrDefault()
			?? throw new KeyNotFoundException($"{fileName} not found below {root.FullName}");
	}

	public static void Save(this FileInfo file, string contents)
	{
		using var stream = file.Open(FileMode.Open, FileAccess.Write, FileShare.None);
		using var writer = new StreamWriter(stream);
		writer.Write(contents);
	}

	public static bool IsTestProject(this Models.Generated.ProjectType project)
	{
		if (project is null) throw new ArgumentNullException(nameof(project));
		if (project.ItemGroup is null) return false;

		foreach (var group in project.ItemGroup)
		{
			if (group?.PackageReference is null) continue;

			foreach (var reference in group.PackageReference)
			{
				if (reference is null) continue;
				if (reference?.Include?.Equals("Microsoft.NET.Test.Sdk", StringComparison.OrdinalIgnoreCase) ?? false) return true;
			}
		}

		return false;
	}

	public static int IndexOf<T>(this IEnumerable<T> list, T find)
	{
		var tuples = list.Select((item, index) => (item, index));

		var query = from tuple in tuples
					where Equals(tuple.item, find)
					select tuple.index;

		return query.First();
	}
}
