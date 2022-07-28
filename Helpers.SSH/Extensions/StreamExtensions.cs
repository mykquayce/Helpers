namespace System.IO;

public static class StreamExtensions
{
	public async static IAsyncEnumerable<string> ReadLinesAsync(this Stream stream, CancellationToken? cancellationToken = null)
	{
		using var reader = new StreamReader(stream, leaveOpen: true);

		for (string? line = await f(); line is not null; line = await f())
		{
			yield return line;
		}

		ValueTask<string?> f() => reader!.ReadLineAsync(cancellationToken ?? CancellationToken.None);
	}
}
