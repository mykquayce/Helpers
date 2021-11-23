using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Helpers.DockerHub;

public record Config(
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri AuthApiBaseAddress,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri RegistryApiBaseAddress,
	string Username,
	string Password)
	: IOptions<Config>
{
	public readonly static Uri DefaultAuthApiBaseAddress = new("https://auth.docker.io", UriKind.Absolute);
	public readonly static Uri DefaultRegistryApiBaseAddress = new("https://registry-1.docker.io", UriKind.Absolute);

	public Config(string username, string password)
		: this(DefaultAuthApiBaseAddress, DefaultRegistryApiBaseAddress, username, password)
	{ }

	public string Credentials => string.Join(':', Username, Password).ToBase64String();


	#region ioptions implementation
	public Config Value => this;
	#endregion ioptions implementation
}
