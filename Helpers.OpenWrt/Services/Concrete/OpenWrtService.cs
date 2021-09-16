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
			_openWrtClient = Guard.Argument(openWrtClient).NotNull().Value;
		}

		public Task AddBlackholeAsync(Helpers.Networking.Models.AddressPrefix prefix)
		{
			Guard.Argument(prefix).NotNull();
			return _openWrtClient.ExecuteCommandAsync("ip route add blackhole " + prefix);
		}

		public async Task AddBlackholesAsync(IEnumerable<Networking.Models.AddressPrefix> prefixes)
		{
			Guard.Argument(prefixes).NotNull();

			foreach (var prefix in prefixes)
			{
				await AddBlackholeAsync(prefix);
			}
		}

		public async IAsyncEnumerable<Helpers.Networking.Models.AddressPrefix> GetBlackholesAsync()
		{
			var response = await _openWrtClient.ExecuteCommandAsync("ip route show");

			Guard.Argument(response).NotNull().NotEmpty().NotWhiteSpace();

			var matches = _blackholeRegex.Matches(response);

			foreach (Match match in matches)
			{
				Guard.Argument(match).NotNull();
				var s = match.Groups[1].Value;
				var prefix = Helpers.Networking.Models.AddressPrefix.Parse(s);
				yield return prefix;
			}
		}

		public Task DeleteBlackholeAsync(Helpers.Networking.Models.AddressPrefix blackhole)
		{
			Guard.Argument(blackhole).NotNull();
			return _openWrtClient.ExecuteCommandAsync("ip route delete blackhole " + blackhole);
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
