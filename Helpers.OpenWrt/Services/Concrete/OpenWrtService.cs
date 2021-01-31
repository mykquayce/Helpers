using Dawn;
using System.Collections.Generic;
using System.Linq;
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

		public Task AddBlackholesAsync(IEnumerable<Networking.Models.SubnetAddress> subnetAddresses)
		{
			Guard.Argument(() => subnetAddresses).NotNull();
			var tasks = subnetAddresses.Select(AddBlackholeAsync);
			return Task.WhenAll(tasks);
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
	}
}
