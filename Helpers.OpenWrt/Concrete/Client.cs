using Dawn;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Helpers.OpenWrt.Concrete;

public class Client : Helpers.Web.WebClientBase, IClient
{
	#region config
	public record Config(string EndPoint, string Username, string Password)
		: IOptions<Config>
	{
		public const string DefaultEndPoint = "192.168.1.1";
		public const string DefaultUsername = "root";
		public const string DefaultPassword = "root";

		public Config() : this(DefaultEndPoint, DefaultUsername, DefaultPassword) { }

		public static Config Defaults => new();

		public Config Value => this;
	}
	#endregion config

	private readonly string _username, _password;
	private readonly IMemoryCache _memoryCache;
	private readonly string _tokenCacheKey;

	public Client(HttpClient httpClient, IMemoryCache memoryCache, IOptions<Config> options)
		: base(httpClient)
	{
		_memoryCache = Guard.Argument(memoryCache).NotNull().Value;
		var config = Guard.Argument(options).NotNull().Wrap(o => o.Value).NotNull().Value;

		_username = Guard.Argument(config.Username).NotNull().NotEmpty().NotWhiteSpace().NotEqual("\u2026").Value;
		_password = Guard.Argument(config.Password).NotNull().NotEmpty().NotWhiteSpace().NotEqual("\u2026").Value;
		_tokenCacheKey = config.EndPoint + "-config-key";
	}

	public async Task<string> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default)
	{
		Guard.Argument(command).NotNull().NotEmpty().NotWhiteSpace();
		var token = await GetLoginTokenAsync(cancellationToken);
		var uri = new Uri("/cgi-bin/luci/rpc/sys?auth=" + token, UriKind.Relative);
		var @object = new Models.CommandRequestObject(command);
		return await SendAsync(uri, @object, cancellationToken);
	}

	public async Task<string> GetLoginTokenAsync(CancellationToken cancellationToken = default)
	{
		if (_memoryCache.TryGetValue(_tokenCacheKey, out string? token))
		{
			return token!;
		}

		token = await LoginAsync(cancellationToken);
		_memoryCache.Set(_tokenCacheKey, token, TimeSpan.FromHours(1));
		return token;
	}

	public Task<string> LoginAsync(CancellationToken cancellationToken = default)
	{
		var uri = new Uri("/cgi-bin/luci/rpc/auth", UriKind.Relative);
		var @object = new Models.LoginRequestObject(_username, _password);
		return SendAsync(uri, @object, cancellationToken);
	}

	public async Task<string> SendAsync(Uri requestUri, Models.RequestObject requestObject, CancellationToken cancellationToken = default)
	{
		var body = JsonSerializer.Serialize(requestObject);
		(_, _, var response) = await base.SendAsync<Models.ResponseObject>(HttpMethod.Post, requestUri, body, cancellationToken);
		return response!.result;
	}
}
