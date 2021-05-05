using System;

namespace Helpers.OldhamCouncil.Tests.Fixtures
{
	public sealed class ServiceFixture : IDisposable
	{
		private readonly ClientFixture _clientFixture;

		public ServiceFixture()
		{
			_clientFixture = new ClientFixture();
			var client = _clientFixture.Client;
			Service = new Concrete.Service(client);
		}

		public IService Service { get; }

		public void Dispose() => _clientFixture?.Dispose();
	}
}
