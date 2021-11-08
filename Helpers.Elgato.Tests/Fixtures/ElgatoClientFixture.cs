using System;

namespace Helpers.Elgato.Tests.Fixtures
{
	public sealed class ElgatoClientFixture
	{
		public IElgatoClient Client { get; } = new Concrete.ElgatoClient();
	}
}
