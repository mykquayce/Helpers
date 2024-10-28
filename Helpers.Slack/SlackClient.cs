using Helpers.Slack.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using System.Text.Json;

namespace Helpers.Slack
{
	public class SlackClient : Web.WebClientBase
	{
		private readonly string _token;
		private readonly ICollection<string> _webhookSegments;

		public SlackClient(
			IOptions<Models.Settings> settingsOptions,
			ILogger? logger = default,
			ITracer? tracer = default)
			: base(new ClientFactory(), logger, tracer)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(settingsOptions?.Value?.Token);
			ArgumentOutOfRangeException.ThrowIfZero(settingsOptions?.Value?.WebhookSegments?.Length ?? 0);

			_token = settingsOptions!.Value!.Token;
			_webhookSegments = settingsOptions!.Value!.WebhookSegments!;
		}

		public async Task<bool> SendTextAsync(string text)
		{
			var requestStream = new MemoryStream();

			await JsonSerializer.SerializeAsync(requestStream, new { text, });

			var requestUriString = "/services/" + string.Join('/', _webhookSegments);
			var requestUri = new Uri(requestUriString, UriKind.Relative);

			var response = await base.SendAsync(HttpMethod.Post, requestUri);

			return response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		public async IAsyncEnumerable<Channel> GetChannelsAsync()
		{
			var uri = new Uri("/api/conversations.list?token=" + _token, UriKind.Relative);

			var response = await base.SendAsync<Models.ChannelsResponse>(HttpMethod.Get, uri);

			foreach (var channel in response.Object!.Channels!)
			{
				yield return channel;
			}
		}

		private sealed class ClientFactory : IHttpClientFactory, IDisposable
		{
			private readonly HttpMessageHandler _handler;
			private readonly System.Net.Http.HttpClient _httpClient;

			public ClientFactory()
			{
				_handler = new HttpClientHandler
				{
					AllowAutoRedirect = false,
				};

				_httpClient = new System.Net.Http.HttpClient(_handler)
				{
					BaseAddress = new Uri("https://slack.com", UriKind.Absolute),
				};
			}

			public System.Net.Http.HttpClient CreateClient(string name)
			{
				return _httpClient;
			}

			public void Dispose()
			{
				_httpClient?.Dispose();
				_handler?.Dispose();
			}
		}
	}
}
