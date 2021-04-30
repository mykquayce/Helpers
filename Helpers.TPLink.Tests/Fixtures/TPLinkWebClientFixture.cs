using System;

namespace Helpers.TPLink.Tests.Fixtures
{
	public sealed class TPLinkWebClientFixture : UserSecretsFixture, IDisposable
	{
		public TPLinkWebClientFixture()
		{
			//var config = new Concrete.TPLinkWebClient.Config(base.Username, base.Password);
			TPLinkWebClient = new Helpers.TPLink.Concrete.TPLinkWebClient(base.Config);
		}

		public Helpers.TPLink.ITPLinkWebClient TPLinkWebClient { get; }

		public void Dispose() => TPLinkWebClient.Dispose();
	}
}
