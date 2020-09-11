using System;
using Xunit;

namespace Helpers.Steam.Tests.Fixtures
{
	public class HttpClientFixture : IClassFixture<Helpers.XUnitClassFixtures.HttpClientFixture>
	{
		private const string _uriString = "https://api.steampowered.com";

		private readonly Helpers.XUnitClassFixtures.HttpClientFixture _fixture
			= new Helpers.XUnitClassFixtures.HttpClientFixture();

		public HttpClientFixture()
		{
			if (_fixture.HttpClient.BaseAddress is null)
			{
				_fixture.HttpClient.BaseAddress = new Uri(_uriString);
			}
		}

		public System.Net.Http.HttpClient HttpClient => _fixture.HttpClient;
	}
}
