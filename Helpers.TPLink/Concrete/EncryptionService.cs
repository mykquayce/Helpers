using Microsoft.Extensions.Options;

namespace Helpers.TPLink.Concrete
{
	public class EncryptionService : IEncryptionService
	{
		#region config poco
		public class Config
		{
			public byte? InitialKey { get; init; } = 0xAB;
		}
		#endregion config poco

		private readonly byte _initialKey;

		#region constructors
		public EncryptionService(IOptions<Config> options)
			: this(options.Value)
		{ }

		public EncryptionService(Config config)
			: this(config.InitialKey ?? default)
		{ }

		public EncryptionService(byte initialKey) => _initialKey = initialKey;
		#endregion constructors

		public byte[] Decrypt(byte[] data)
		{
			var result = new byte[data.Length];
			var key = _initialKey;
			for (var a = 0; a < result.Length; a++)
			{
				result[a] = (byte)(key ^ data[a]);
				key = data[a];
			}
			return result;
		}

		public byte[] Encrypt(byte[] value)
		{
			var result = new byte[value.Length];
			var key = _initialKey;
			for (var a = 0; a < result.Length; a++)
			{
				key = result[a] = (byte)(key ^ value[a]);
			}
			return result;
		}
	}
}
