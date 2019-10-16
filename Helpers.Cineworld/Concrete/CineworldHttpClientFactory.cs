using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Helpers.Cineworld.Concrete
{
	public class CineworldHttpClientFactory : IHttpClientFactory
	{
		public System.Net.Http.HttpClient CreateClient(string name)
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };

			return new System.Net.Http.HttpClient(handler)
			{
				BaseAddress = Settings.BaseAddress,
				DefaultRequestHeaders =
				{
					Accept =
					{
						new MediaTypeWithQualityHeaderValue("application/xml"),
					},
				},
				Timeout = TimeSpan.FromSeconds(10),
			};
		}
	}
}
