using System;

namespace Helpers.Elgato.Tests.Fixtures
{
	public sealed class ElgatoClientFixture : IDisposable
	{
		public IElgatoClient Client { get; } = new Concrete.ElgatoClient();

		public void Dispose() => Client?.Dispose();
	}
}
