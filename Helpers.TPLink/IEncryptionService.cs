namespace Helpers.TPLink
{
	public interface IEncryptionService
	{
		byte[] Decrypt(byte[] data);
		byte[] Encrypt(byte[] value);
	}
}
