using System.Text.RegularExpressions;
using Xunit;

namespace Helpers.Common.Tests;

public class SolutionTests
{
	[Theory]
	[InlineData("Helpers")]
	public void FindSolutionRootTests(string expected)
	{
		var root = FindSolutionRoot();
		Assert.NotNull(root);
		Assert.Equal(expected, root!.Name);
	}

	public static DirectoryInfo FindSolutionRoot() => FindSolutionRoot(new DirectoryInfo(Environment.CurrentDirectory));

	public static DirectoryInfo FindSolutionRoot(DirectoryInfo directory)
	{
		DirectoryInfo? curr = directory;

		while (directory is not null && !curr!.EnumerateFiles("*.sln").Any())
		{
			curr = curr.Parent;
		}

		return curr ?? throw new Exception("solution root not found");
	}

	[Fact]
	public async Task GetDeploymentPhasesTests()
	{
		var phases = await GetDeploymentPhasesAsync().ToListAsync();

		Assert.NotEmpty(phases);
		Assert.All(phases, Assert.NotNull);
		Assert.All(phases, Assert.NotEmpty);

		var s = string.Join(Environment.NewLine, from ss in phases
												 select string.Join(", ", from s in ss
																		  orderby s
																		  select s[8..]));
	}

	private async static IAsyncEnumerable<IReadOnlyCollection<string>> GetDeploymentPhasesAsync()
	{
		IReadOnlyDictionary<string, IReadOnlyCollection<string>> projectDependencies;
		{
			var root = FindSolutionRoot();
			projectDependencies = await GetSolutionProjectDependenciesAsync(root)
				.ToDictionaryAsync(kvp => kvp.Key, kvp => kvp.Value);
		}

		ICollection<ICollection<string>> phases = new List<ICollection<string>>();

		while (phases.Sum(ss => ss.Count) < projectDependencies.Count)
		{
			var phase = new List<string>();
			var soFar = phases.SelectMany(s => s).ToList();

			foreach (var (project, dependencies) in projectDependencies)
			{
				if (soFar.Contains(project)) continue;
				if (dependencies.Except(soFar).Any()) continue;

				phase.Add(project);
			}

			phases.Add(phase);
			yield return phase.AsReadOnly();
		}
	}

	private async static IAsyncEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>> GetSolutionProjectDependenciesAsync(DirectoryInfo root)
	{
		var files = from f in root.EnumerateFiles("*.csproj", SearchOption.AllDirectories)
					where !f.Name.EndsWith(".Tests.csproj", StringComparison.OrdinalIgnoreCase)
					select f;

		foreach (var file in files)
		{
			var (project, dependencies) = await GetProjectDependenciesAsync(file);

			yield return new(project, dependencies);
		}
	}

	private const RegexOptions _regexOptions = RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline;
	private static readonly Regex _regex = new(@"<PackageReference Include=""(.+?)""", _regexOptions);

	private async static Task<KeyValuePair<string, IReadOnlyCollection<string>>> GetProjectDependenciesAsync(FileInfo projectFile)
	{
		string xml;
		{
			await using var stream = projectFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			using var reader = new StreamReader(stream);
			xml = await reader.ReadToEndAsync();
		}

		var dependencies = from m in _regex.Matches(xml)
						   from g in m.Groups.Values.Skip(1)
						   let d = g.Value
						   where d.StartsWith("Helpers.", StringComparison.OrdinalIgnoreCase)
						   select d;

		var project = Path.GetFileNameWithoutExtension(projectFile.Name);

		return new(project, dependencies.ToList().AsReadOnly());
	}
}
