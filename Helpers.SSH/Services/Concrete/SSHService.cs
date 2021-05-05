using Dawn;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers.SSH.Services.Concrete
{
	public class SSHService : ISSHService
	{
		#region Config
		public record Config(string Host, int Port, string Username, string? Password = default, string? PathToPublicKey = default, string? PathToPrivateKey = default)
		{
			public const string DefaultHost = "localhost";
			public const int DefaultPort = 22;
			public const string DefaultUsername = "root";

			public Config() : this(DefaultHost, DefaultPort, DefaultUsername) { }
		}
		#endregion Config

		private readonly Clients.ISSHClient _sshClient;
		private readonly string _newline;

		public SSHService(IOptions<Config> options) : this(options.Value) { }

		public SSHService(Config config)
		{
			if (!string.IsNullOrWhiteSpace(config.Password))
			{
				_sshClient = new Clients.Concrete.SSHClient(config.Host, config.Port, config.Username, config.Password);
			}
			else if (!string.IsNullOrWhiteSpace(config.PathToPrivateKey))
			{
				var path = FixPath(config.PathToPrivateKey);
				var privateKeyFileInfo = new FileInfo(path);
				_sshClient = new Clients.Concrete.SSHClient(config.Host, config.Port, config.Username, privateKeyFileInfo);
			}
			else
			{
				_sshClient = new Clients.Concrete.SSHClient(config.Host, config.Port, config.Username);
			}

			_newline = _sshClient.RunCommandAsync("echo").GetAwaiter().GetResult();
		}

		public static string FixPath(string path)
		{
			Guard.Argument(() => path).NotNull().NotEmpty().NotWhiteSpace();

			if (!path.StartsWith('~'))
			{
				return path;
			}

			var pwd = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify);
			var paths = (pwd + path[1..]).Split('/', '\\');
			return Path.Combine(paths);
		}

		public async IAsyncEnumerable<Helpers.Networking.Models.SubnetAddress> GetBlackholesAsync()
		{
			var command = "(ip route show && ip -6 route show) | grep ^[Bb]lackhole | awk '{print($2)}'";

			var response = await _sshClient.RunCommandAsync(command);

			var lines = response.Split(_newline, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				var subnetAddress = Helpers.Networking.Models.SubnetAddress.Parse(line);
				yield return subnetAddress;
			}
		}

		public Task AddBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress)
		{
			Guard.Argument(() => subnetAddress).NotNull();
			return _sshClient.RunCommandAsync("ip route add blackhole " + subnetAddress);
		}

		public Task AddBlackholesAsync(IEnumerable<Networking.Models.SubnetAddress> subnetAddresses)
			=> Task.WhenAll(subnetAddresses.Select(AddBlackholeAsync));

		public Task DeleteBlackholeAsync(Helpers.Networking.Models.SubnetAddress subnetAddress)
		{
			Guard.Argument(() => subnetAddress).NotNull();
			return _sshClient.RunCommandAsync("ip route delete blackhole " + subnetAddress);
		}

		public Task DeleteBlackholesAsync(IEnumerable<Networking.Models.SubnetAddress> subnetAddresses)
			=> Task.WhenAll(subnetAddresses.Select(DeleteBlackholeAsync));

		public Task DeleteBlackholesAsync()
			=> _sshClient.RunCommandAsync("(ip route show && ip -6 route show) | grep ^blackhole | awk '{system(\"ip route delete blackhole \" $2)}'");

		public async IAsyncEnumerable<Helpers.Networking.Models.DhcpEntry> GetDhcpLeasesAsync()
		{
			var output = await _sshClient.RunCommandAsync("cat /tmp/dhcp.leases");

			var lines = output.Split(_newline, StringSplitOptions.RemoveEmptyEntries);

			foreach (var line in lines)
			{
				yield return Helpers.Networking.Models.DhcpEntry.Parse(line);
			}
		}

		#region Dispose pattern
		private bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					_sshClient?.Dispose();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion Dispose pattern
	}
}
