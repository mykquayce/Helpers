using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace System.Net.Http;

public class RetryHandler(ILogger<RetryHandler> logger, IOptions<RetryHandler.Config> config) : DelegatingHandler
{
	public class Config
	{
		public short Count { get; set; }
		public TimeSpan Pause { get; set; }
	}

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(config?.Value, nameof(config));
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(config.Value.Count, nameof(Config.Count));
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(config.Value.Pause, TimeSpan.Zero, nameof(Config.Pause));

		return Policy
			.Handle<HttpRequestException>()
			.Or<TaskCanceledException>()
			.OrResult<HttpResponseMessage>(x =>
			{
				return (int)x.StatusCode switch
				{
					>= 200 and <= 299 => false,
					>= 300 and <= 399 => false,
					400 => false, // BadRequest
					404 => false,
					_ => true,
				};
			})
			.WaitAndRetryAsync(retryCount: config.Value.Count, _ => config.Value.Pause)
			.ExecuteAsync(async () =>
			{
				HttpResponseMessage response;
				try
				{
					response = await base.SendAsync(request, cancellationToken);
				}
				catch (Sockets.SocketException ex) when (ex.Message.StartsWith("Connection refused", StringComparison.OrdinalIgnoreCase))
				{
					throw;
				}
				var content = await response.Content.ReadAsStringAsync(cancellationToken);
				logger.LogWarning("{uri}{newline}{response}{newline}{content}", request?.RequestUri, Environment.NewLine, response, Environment.NewLine, content);
				return response;
			});
	}
}
