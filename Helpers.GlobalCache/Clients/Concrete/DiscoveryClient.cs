using Dawn;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.GlobalCache.Clients.Concrete
{
	public class DiscoveryClient : IDiscoveryClient
	{
		private readonly Networking.Clients.IUdpClient _udpClient;

		public DiscoveryClient(Helpers.Networking.Clients.IUdpClient udpClient)
		{
			_udpClient = Guard.Argument(() => udpClient).NotNull().Value;
		}

		public IAsyncEnumerable<Models.Beacon> DiscoverAsync()
			=> _udpClient.DiscoverAsync().Select(Models.Beacon.Parse).Distinct();

		#region IDisposable implementation
		private bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					_udpClient?.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposed = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~DiscoveryClient()
		// {
		//     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			System.GC.SuppressFinalize(this);
		}
		#endregion IDisposable implementation
	}
}
