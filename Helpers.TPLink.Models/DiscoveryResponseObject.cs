using System.Net;
using System.Net.NetworkInformation;

namespace Helpers.TPLink.Models
{
#pragma warning disable IDE1006 // Naming Styles
	public record DiscoveryResponseObject(
		DiscoveryResponseObject.ResultObject? result,
		int? error_code)
	{
		public DiscoveryResponseObject() : this(default, default) { }

		public IPAddress IPAddress => IPAddress.Parse(result!.ip!);
		public PhysicalAddress PhysicalAddress => PhysicalAddress.Parse(result!.mac!);

		public record ResultObject(
			string? ip,
			string? mac,
			string? device_id,
			string? owner,
			string? device_type,
			string? device_model,
			string? hw_ver,
			bool? factory_default,
			ResultObject.EncryptionSchemeObject? mgt_encrypt_schm)
		{
			public ResultObject() : this(default, default, default, default, default, default, default, default, default) { }

			public record EncryptionSchemeObject(
				bool? is_support_https,
				string? encrypt_type,
				int? http_port)
			{
				public EncryptionSchemeObject() : this(default, default, default) { }
			}
		}
	}
#pragma warning restore IDE1006 // Naming Styles
}
