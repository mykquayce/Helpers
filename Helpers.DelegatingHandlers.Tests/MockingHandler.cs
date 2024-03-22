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
		var match = BytesRegex().Match(request.RequestUri?.PathAndQuery ?? string.Empty);

		if (match.Success)
		{
			var count = short.Parse(match.Groups[1].Value);
			var bytes = new byte[count];
			rng.GetBytes(bytes);
			return new(HttpStatusCode.OK) { Content = new ByteArrayContent(bytes), };
		}

		return await base.SendAsync(request, cancellationToken);
	}

	[GeneratedRegex(@"^\/bytes\/(\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
	private static partial Regex BytesRegex();
}
