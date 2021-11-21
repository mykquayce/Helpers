using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace Helpers.DockerHub;

public record Config(
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri AuthApiBaseAddress,
	[property: JsonConverter(typeof(Helpers.Json.Converters.JsonUriConverter))] Uri RegistryApiBaseAddress,
	string Username,
	string Password,
	string Organization,
	string Repository,
	[property: JsonConverter(typeof(JsonStringEnumConverter))] Config.Scopes Scope)
	: IOptions<Config>
{
	public readonly static Uri DefaultAuthApiBaseAddress = new("https://auth.docker.io", UriKind.Absolute);
	public readonly static Uri DefaultRegistryApiBaseAddress = new("https://registry-1.docker.io", UriKind.Absolute);

	public Config(string username, string password, string organization, string repository, Scopes scope)
		: this(DefaultAuthApiBaseAddress, DefaultRegistryApiBaseAddress, username, password, organization, repository, scope)
	{ }

	public string Credentials => string.Join(':', Username, Password).ToBase64String();
	public string RepositoryScope => $"repository:{Organization}/{Repository}:{Scope.ToString().ToLowerInvariant()}";

	[Flags]
	public enum Scopes : byte
	{
		Pull = 1,
		Push = 2,
	}

	#region ioptions implementation
	public Config Value => this;
	#endregion ioptions implementation
}
