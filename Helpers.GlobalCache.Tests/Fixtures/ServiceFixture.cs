using System;

namespace Helpers.GlobalCache.Tests.Fixtures
{
	public sealed class ServiceFixture : IDisposable
	{
		public void Dispose() => Service.Dispose();

		public IService Service { get; } = new Concrete.Service();
	}
}
