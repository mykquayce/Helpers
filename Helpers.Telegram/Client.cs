using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers.Telegram
{
	public static class Client
	{
		private static readonly Regex _apiKeyRegex = new(@"^\d+:[-\w]+$", RegexOptions.Compiled);
		private static readonly Uri _baseAddress = new("https://api.telegram.org/", UriKind.Absolute);
		private static readonly HttpMessageInvoker _httpClient;// = new HttpClient { BaseAddress = _baseAddress, };

		private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
		{
			AllowTrailingCommas = true,
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			WriteIndented = true,
		};

		static Client()
		{
			//var header = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");

			//var coll = new System.Net.Http.Headers.HttpHeaderValueCollection<System.Net.Http.Headers.MediaTypeWithQualityHeaderValue>()

			_httpClient = new HttpClient
			{
				BaseAddress = _baseAddress,
				/*DefaultRequestHeaders =
				{
					Accept =
					{
						new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("")
					},
					UserAgent =
					{
						new System.Net.Http.Headers.ProductInfoHeaderValue(new System.Net.Http.Headers.ProductHeaderValue("curl", "7.60.0")),
					},
					Host = "api.telegram.org",
				}*/
			};

			//((HttpClient)_httpClient).DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"))
		}

		public async static Task<int> SendMesssageAsync(string apiKey, int chatId, string message)
		{
			if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
			if (chatId == default) throw new ArgumentNullException(nameof(chatId));
			if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

			if (!_apiKeyRegex.IsMatch(apiKey))
			{
				throw new ArgumentOutOfRangeException(nameof(apiKey), apiKey, $"Unexpected API key {apiKey}, should match: {_apiKeyRegex}")
				{
					Data = { [nameof(apiKey)] = apiKey, },
				};
			}

			var requestUri = new Uri($"/bot{apiKey}/sendMessage", UriKind.Relative);

			var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

			var body = $"chat_id={chatId:D}&text={message}";

			var requestContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded")
			/*{
				Headers =
				{
					ContentLength = body.Length,
					ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded"),
				},
			}*/;

			httpRequestMessage.Content = requestContent;

			var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, CancellationToken.None);

			using var content = await httpResponseMessage.Content.ReadAsStreamAsync();

			if (httpResponseMessage.IsSuccessStatusCode)
			{
				var response = await JsonSerializer.DeserializeAsync<Models.Generated.Response>(content, _jsonSerializerOptions);

				if (response?.Ok == true
					&& response!.Result is not null)
				{
					return response.Result.MessageId;
				}
			}

			throw new Exception("Unexpected response from API")
			{
				Data =
				{
					[nameof(HttpResponseMessage.StatusCode)] = httpResponseMessage.StatusCode,
					[nameof(HttpResponseMessage.Content)] = content,
				},
			};
		}
	}
}
