using Microsoft.Extensions.Logging;
using System.Text;

namespace System.Net.Http;

public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var response = await base.SendAsync(request, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			var sb = new StringBuilder();

			sb.AppendLine("response: " + response.StatusCode.ToString("G"));
			if (response.Content != null)
			{
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				if (!string.IsNullOrEmpty(content)) { sb.AppendLine(content); }
			}
			sb.AppendJoin(Environment.NewLine, from kvp in response.Headers
											   select kvp.Key + ":" + string.Join(',', kvp.Value));

			sb.AppendLine();

			sb.AppendLine("request: " + request.Method + " " + request.RequestUri?.OriginalString);
			if (request.Content != null)
			{
				var content = await request.Content.ReadAsStringAsync(cancellationToken);
				if (!string.IsNullOrEmpty(content)) { sb.AppendLine(content); }
			}
			sb.AppendJoin(Environment.NewLine, from kvp in request.Headers
											   select kvp.Key + ":" + string.Join(',', kvp.Value));

			logger.LogError("{error}", sb.ToString());
		}

		return response;
	}
}
