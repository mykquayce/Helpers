using System;
using System.Net.Http;
using System.Xml.Serialization;

namespace Helpers.Reddit.Tests.Fixtures
{
	public sealed class RedditClientFixture : IDisposable
	{
		public RedditClientFixture()
		{
			var baseAddress = new Uri("https://old.reddit.com", UriKind.Absolute);
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			var httpClient = new HttpClient(handler) { BaseAddress = baseAddress, };

			var xmlSerializerFactory = new XmlSerializerFactory();

			RedditClient = new Helpers.Reddit.Concrete.RedditClient(httpClient, xmlSerializerFactory);
		}

		public Helpers.Reddit.IRedditClient RedditClient { get; }

		public void Dispose() => RedditClient.Dispose();
	}
}
