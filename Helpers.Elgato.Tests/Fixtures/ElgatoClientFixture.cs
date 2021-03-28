using Helpers.Elgato.Clients.Concrete;
using System;
using System.Net.Http;

namespace Helpers.Elgato.Tests.Fixtures
{
	public class ElgatoClientFixture : IDisposable
	{
		private readonly HttpClient _httpClient;

		public ElgatoClientFixture()
		{
			var userSecretsFixture = new Helpers.XUnitClassFixtures.UserSecretsFixture();
			var host = userSecretsFixture["Elgato:EndPoint:IPAddress"];
			var config = new ElgatoClient.Config(Host: host);
			_httpClient = new HttpClient { BaseAddress = config.Uri, };
			Client = new ElgatoClient(_httpClient);
		}

		public Helpers.Elgato.Clients.IElgatoClient Client { get; }

		#region dispose
		private bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_httpClient.Dispose();
					Client.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion dispose
	}
}
