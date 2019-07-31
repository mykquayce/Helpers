using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Helpers.Discord.Concrete
{
	public class DiscordClient : Helpers.HttpClient.HttpClientBase, IDiscordClient
	{
		private readonly Uri _relativeUri;

		public DiscordClient(
			IHttpClientFactory httpClientFactory,
			IOptions<Models.Webhook> webhookOptions,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(httpClientFactory, logger, tracer)
		{
			var webhook = Guard.Argument(() => webhookOptions).NotNull().Value.Value;

			Guard.Argument(() => webhook).NotNull();
			Guard.Argument(() => webhook.Id).InRange(1, long.MaxValue);
			Guard.Argument(() => webhook.Token).NotNull().NotEmpty().NotWhiteSpace().Matches(@"^[0-9A-Za-z_]+$");

			_relativeUri = new Uri($"/api/webhooks/{webhook.Id:D}/{webhook.Token}", UriKind.Relative);
		}

		public Task SendMessageAsync(string content)
		{
			Guard.Argument(() => content).NotNull().NotEmpty().NotWhiteSpace().MaxLength(2_000);

			var body = System.Text.Json.JsonSerializer.Serialize(new { content, });

			return base.SendAsync(HttpMethod.Post, _relativeUri, body);
		}
	}
}
