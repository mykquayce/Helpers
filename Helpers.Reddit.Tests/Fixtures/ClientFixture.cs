using System;
using System.Net.Http;
using System.Xml.Serialization;

namespace Helpers.Reddit.Tests.Fixtures
{
	public sealed class ClientFixture : IDisposable
	{
		public ClientFixture()
		{
			var baseAddress = new Uri("https://old.reddit.com", UriKind.Absolute);
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			var httpClient = new HttpClient(handler) { BaseAddress = baseAddress, };

			var xmlSerializerFactory = new XmlSerializerFactory();

			RedditClient = new Helpers.Reddit.Concrete.Client(httpClient, xmlSerializerFactory);
		}

		public Helpers.Reddit.IClient RedditClient { get; }

		public void Dispose() => RedditClient.Dispose();
	}
}
