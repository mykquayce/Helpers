using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;

namespace Helpers.Discord.Concrete
{
	public class DiscordClient : Helpers.Web.WebClientBase, IDiscordClient
	{
		private readonly Uri _relativeUri;

		public DiscordClient(
			IHttpClientFactory httpClientFactory,
			IOptions<Models.Webhook> webhookOptions,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(httpClientFactory, logger, tracer)
		{
			ArgumentNullException.ThrowIfNull(webhookOptions?.Value);

			var webhook = webhookOptions!.Value;

			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(webhook.Id ?? 0);
			ArgumentException.ThrowIfNullOrWhiteSpace(webhook.Token);

			_relativeUri = new Uri($"/api/webhooks/{webhook.Id:D}/{webhook.Token}", UriKind.Relative);
		}

		public Task SendMessageAsync(string content)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(content);

			var body = System.Text.Json.JsonSerializer.Serialize(new { content, });

			return base.SendAsync(HttpMethod.Post, _relativeUri, body);
		}
	}
}
