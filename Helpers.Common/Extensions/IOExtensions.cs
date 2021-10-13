using Dawn;
using System.Text;

namespace System.IO;

public static class IOExtensions
{
	public static IEnumerable<string> ReadLines(this Stream stream)
	{
		var reader = new StreamReader(stream, Encoding.UTF8);
		return reader.ReadLines();
	}

	public static IAsyncEnumerable<string> ReadLinesAsync(this Stream stream)
	{
		var reader = new StreamReader(stream, Encoding.UTF8);
		return reader.ReadLinesAsync();
	}

	public static IEnumerable<string> ReadLines(this StreamReader reader)
	{
		string? line;
		while ((line = reader.ReadLine()) is not null)
		{
			yield return line;
		}
	}

	public async static IAsyncEnumerable<string> ReadLinesAsync(this StreamReader reader)
	{
		string? line;
		while ((line = await reader.ReadLineAsync()) is not null)
		{
			yield return line;
		}
	}

	public static void EnsureExists(this DirectoryInfo dir)
	{
		Guard.Argument(dir).NotNull();
		if (dir.Exists) return;

		var stack = new Stack<DirectoryInfo>();
		var curr = dir;

		while (curr?.Exists == false)
		{
			stack.Push(curr);
			curr = curr.Parent;
		}

		while (stack.TryPop(out curr))
		{
			curr.Parent!.CreateSubdirectory(curr.Name);
		}
	}

	public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfosLeafFirst(this DirectoryInfo dir, string searchPattern = "*.*")
	{
		foreach (var fsi in dir.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
		{
			if (fsi is DirectoryInfo subDirectory)
			{
				foreach (var nested in subDirectory.EnumerateFileSystemInfosLeafFirst(searchPattern))
				{
					yield return nested;
				}
			}

			yield return fsi;
		}
	}
}
