using Helpers.Nanoleaf.Models;

namespace Helpers.Nanoleaf;

public interface IClient
{
	Task<InfoResponse> GetInfoAsync(CancellationToken cancellationToken = default);
	Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
	Task<HttpResponseMessage> SetEffectAsync(string effect, CancellationToken cancellationToken = default);
	Task<HttpResponseMessage> SetOnAsync(bool value, CancellationToken cancellationToken = default);
}
