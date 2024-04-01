using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace System.Net.Http;

public partial class MockingHandler(RandomNumberGenerator rng) : DelegatingHandler
{
	public MockingHandler()
		: this(RandomNumberGenerator.Create())
	{ }

	protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var path = request.RequestUri?.PathAndQuery ?? string.Empty;

		if (string.Equals("/headers", path, StringComparison.OrdinalIgnoreCase))
		{
			var headers = request.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			var response = new { headers, };
			return new(HttpStatusCode.OK) { Content = JsonContent.Create(response), };
		}

		var bytesMatch = BytesRegex().Match(path);

		if (bytesMatch.Success)
		{
			var count = short.Parse(bytesMatch.Groups[1].Value);
			var bytes = new byte[count];
			rng.GetBytes(bytes);
			return new(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes), };
		}

		var statusMatch = StatusRegex().Match(path);

		if (statusMatch.Success)
		{
			var code = (HttpStatusCode)short.Parse(statusMatch.Groups[1].Value);
			return new(code) { Content = new StringContent(code.ToString("G")), };
		}

		return await base.SendAsync(request, cancellationToken);
	}

	[GeneratedRegex(@"^\/bytes\/(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
	private static partial Regex BytesRegex();

	[GeneratedRegex(@"^\/status\/(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
	private static partial Regex StatusRegex();
}
