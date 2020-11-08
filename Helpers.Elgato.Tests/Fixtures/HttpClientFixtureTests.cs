using Xunit;

namespace Helpers.Elgato.Tests.Fixtures
{
	public class HttpClientFixtureTests
	{
		[Fact]
		public void BaseAddress_IsNotNullOrEmpty()
		{
			using var fixture = new Fixtures.HttpClientFixture();

			Assert.NotNull(fixture.HttpClient.BaseAddress?.OriginalString);
			Assert.NotEmpty(fixture.HttpClient.BaseAddress?.OriginalString);
		}
	}
}
