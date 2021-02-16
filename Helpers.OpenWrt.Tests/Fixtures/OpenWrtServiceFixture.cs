using System;

namespace Helpers.OpenWrt.Tests.Fixtures
{
	public sealed class OpenWrtServiceFixture : IDisposable
	{
		public OpenWrtServiceFixture()
		{
			var clientFixture = new OpenWrtClientFixture();

			OpenWrtService = new Services.Concrete.OpenWrtService(clientFixture.OpenWrtClient);
		}

		public Services.IOpenWrtService OpenWrtService { get; }

		public void Dispose() => OpenWrtService.Dispose();
	}
}
