using System.Diagnostics;
using System.Xml.Serialization;
using Xunit;

namespace Helpers.Common.Tests;

public class SolutionTests
{
	private readonly static XmlSerializerFactory _xmlSerializerFactory = new();

	[Theory]
	[InlineData("Helpers")]
	public DirectoryInfo FindSolutionRootTests(string expected)
	{
		var root = FindSolutionRoot();
		Assert.NotNull(root);
		Assert.Equal(expected, root!.Name);
		return root;
	}

	public static DirectoryInfo FindSolutionRoot()
	{
		var curr = new DirectoryInfo(Environment.CurrentDirectory);

		while (curr is not null && !curr.EnumerateFiles("*.sln").Any())
		{
			curr = curr.Parent;
		}

		return curr!;
	}

	[Fact]
	public void DiscoveryHierachy()
	{
		var dictionary = new Dictionary<string, ICollection<string>>(StringComparer.InvariantCultureIgnoreCase);
		var directory = FindSolutionRoot();
		var files = GetProjects(directory);

		foreach (var file in files)
		{
			var kvp = GetProjectDetails(file);
			dictionary.Add(kvp.Key, kvp.Value);
		}
		var output = dictionary.Select(kvp => kvp.Key + "=>" + string.Join(",", kvp.Value)).ToList();

		// find all projects with dependencies - that's layer 0
		var layer0 = dictionary.Where(kvp => !kvp.Value.Any()).Select(kvp => kvp.Key).ToList();

		// find remaining projects with only layer 0 denpendencies - that's layer 1
		var layer1 = (from kvp in dictionary
					  let project = kvp.Key
					  where !layer0.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !kvp.Value.Except(layer0).Any()
					  select project
					).ToList();

		// find remaining projects with only layer 0/1 denpendencies - that's layer 2
		var layer2 = (from kvp in dictionary
					  let project = kvp.Key
					  where !layer0.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !layer1.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !kvp.Value.Except(layer0).Except(layer1).Any()
					  select project
					).ToList();

		// find remaining projects with only layer 0/1/2 denpendencies - that's layer 3
		var layer3 = (from kvp in dictionary
					  let project = kvp.Key
					  where !layer0.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !layer1.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !layer2.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !kvp.Value.Except(layer0).Except(layer1).Except(layer2).Any()
					  select project
					).ToList();

		// find remaining projects with only layer 0/1/2/3 denpendencies - that's layer 4
		var layer4 = (from kvp in dictionary
					  let project = kvp.Key
					  where !layer0.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !layer1.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !layer2.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !layer3.Contains(project, StringComparer.InvariantCultureIgnoreCase)
					  where !kvp.Value.Except(layer0).Except(layer1).Except(layer2).Except(layer3).Any()
					  select project
					).ToList();

		foreach (var layer in new[] { layer0, layer1, layer2, layer3, layer4, })
		{
			var m = string.Join(", ", layer);
			Console.WriteLine(m);
			Debug.WriteLine(m);
		}
	}

	public static IEnumerable<FileInfo> GetProjects(DirectoryInfo directory)
	{
		var options = new EnumerationOptions { MaxRecursionDepth = 1, RecurseSubdirectories = true, ReturnSpecialDirectories = false, };
		Assert.True(directory.Exists);
		return directory.EnumerateFiles("*.csproj", options)
			.Where(fi => !fi.Name.EndsWith(".tests.csproj", StringComparison.InvariantCultureIgnoreCase));
	}

	public static KeyValuePair<string, ICollection<string>> GetProjectDetails(FileInfo file)
	{
		using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
		var project = Deserialize<Models.Generated.ProjectType>(stream);

		IEnumerable<string> query;

		if (project?.ItemGroup is not null)
		{
			query = from ig in project!.ItemGroup
					where ig.PackageReference is not null
					from pr in ig.PackageReference
					let dependency = pr.Include
					where dependency.StartsWith("helpers.", StringComparison.InvariantCultureIgnoreCase)
					select dependency[8..];
		}
		else
		{
			query = Enumerable.Empty<string>();
		}

		var key = Path.GetFileNameWithoutExtension(file.Name)![8..];
		var dependencies = new List<string>(query);

		return new(key, dependencies);
	}

	public static T? Deserialize<T>(Stream stream) where T : class
	{
		return _xmlSerializerFactory
			.CreateSerializer(typeof(T))
			.Deserialize(stream) as T;
	}
}
