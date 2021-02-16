using Dawn;
using Helpers.Web.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers.OpenWrt.Clients.Concrete
{
	public class OpenWrtClient : Helpers.Web.WebClientBase, IOpenWrtClient
	{
		private readonly string _password;
		private string? _token;

		public record Settings(string? EndPoint, string? Password)
		{
			public Settings() : this(default, default) { }
		}

		public OpenWrtClient(HttpClient httpClient, IOptions<Settings> options)
			: base(httpClient)
		{
			_password = Guard.Argument(() => options).NotNull().Wrap(o => o.Value)
				.NotNull().Wrap(s => s.Password!)
				.NotNull().NotEmpty().NotWhiteSpace().NotEqual("\u2026").Value;
		}

		public async Task<string> ExecuteCommandAsync(string command)
		{
			Guard.Argument(() => command).NotNull().NotEmpty().NotWhiteSpace();

			_token ??= await LoginAsync();

			Guard.Argument(() => _token, secure: true)
				.NotNull(message: "token is null")
				.NotEmpty(message: "token is empty")
				.NotWhiteSpace(message: "token is whitespace")
				.Matches("^[0-9a-f]{32}$", message: (_, _) => "token doesn't match regex: ^[0-9a-f]{32}$");

			return await SendAsync("/cgi-bin/luci/rpc/sys?auth=" + _token, "exec", command);
		}

		public Task<string> LoginAsync() => SendAsync("/cgi-bin/luci/rpc/auth", "login", "root", _password);

		public async Task<string> SendAsync(string requestUri, string method, params string[] @params)
		{
			Guard.Argument(() => requestUri).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => method).NotNull().NotEmpty().NotWhiteSpace();
			Guard.Argument(() => @params).NotNull().DoesNotContainNull().DoesNotContain(string.Empty);

			var requestObject = new Models.RequestObject(1, method, @params);
			var requestJson = JsonSerializer.Serialize(requestObject);
			var uri = new Uri(requestUri, UriKind.Relative);
			var response = await base.SendAsync<Models.ResponseObject>(HttpMethod.Post, uri, requestJson);
			var (_, _, _, @object) = response;
			return @object!.result!;
		}
	}
}
