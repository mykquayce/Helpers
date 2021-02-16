using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Helpers.OpenWrt.Tests.Fixtures
{
	public sealed class OpenWrtClientFixture : IDisposable
	{
		public OpenWrtClientFixture()
		{
			var httpClientFixture = new XUnitClassFixtures.HttpClientFixture();
			var userSecretsFixture = new XUnitClassFixtures.UserSecretsFixture();
			Settings = userSecretsFixture.Configuration
				.GetSection("OpenWrt")
				.Get<Clients.Concrete.OpenWrtClient.Settings>()
				?? throw new ArgumentNullException("missing config");

			httpClientFixture.HttpClient.BaseAddress = new Uri("http://" + Settings.EndPoint);

			var options = Options.Create(Settings);

			OpenWrtClient = new Clients.Concrete.OpenWrtClient(httpClientFixture.HttpClient, options);
		}

		public Clients.Concrete.OpenWrtClient.Settings Settings { get; }
		public Clients.IOpenWrtClient OpenWrtClient { get; }

		public void Dispose() => OpenWrtClient.Dispose();
	}
}
