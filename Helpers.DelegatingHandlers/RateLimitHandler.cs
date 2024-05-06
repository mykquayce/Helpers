using System.Threading.RateLimiting;

namespace System.Net.Http;

public class RateLimitHandler(RateLimiter limiter) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		while (true)
		{
			using RateLimitLease lease = await limiter.AcquireAsync(permitCount: 1, cancellationToken);

			if (lease.IsAcquired)
			{
				break;
			}

			await Task.Delay(millisecondsDelay: 200, cancellationToken);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}
