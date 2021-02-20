using Dawn;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers.OpenWrt.Services.Concrete
{
	public class OpenWrtService : IOpenWrtService
	{
		private const RegexOptions _regexOptions = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;
		private static readonly Regex _blackholeRegex = new(@"\bblackhole (\d+\.\d+\.\d+\.\d+(?:\/\d+)?)\b", _regexOptions);

		private readonly Clients.IOpenWrtClient _openWrtClient;

		public OpenWrtService(Clients.IOpenWrtClient openWrtClient)
		{
			_openWrtClient = Guard.Argument(() => openWrtClient).NotNull().Value;
		}

		public Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress)
		{
			Guard.Argument(() => subnetAddress).NotNull();
			return _openWrtClient.ExecuteCommandAsync("ip route add blackhole " + subnetAddress);
		}

		public async Task AddBlackholesAsync(IEnumerable<Networking.Models.SubnetAddress> subnetAddresses)
		{
			Guard.Argument(() => subnetAddresses).NotNull();

			foreach (var subnetAddress in subnetAddresses)
			{
				await AddBlackholeAsync(subnetAddress);
			}
		}

		public async IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync()
		{
			var response = await _openWrtClient.ExecuteCommandAsync("ip route show");

			Guard.Argument(() => response).NotNull().NotEmpty().NotWhiteSpace();

			var matches = _blackholeRegex.Matches(response);

			foreach (Match match in matches)
			{
				Guard.Argument(() => match).NotNull();
				var s = match.Groups[1].Value;
				var subnetAddress = Helpers.Networking.Models.SubnetAddress.Parse(s);
				yield return subnetAddress;
			}
		}

		public Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress)
		{
			Guard.Argument(() => subnetAddress).NotNull();
			return _openWrtClient.ExecuteCommandAsync("ip route delete blackhole " + subnetAddress);
		}

		#region IDisposable implementation
		private bool _disposed;
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
					_openWrtClient.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override finalizer
				// TODO: set large fields to null
				_disposed = true;
			}
		}

		// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		// ~OpenWrtService()
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
