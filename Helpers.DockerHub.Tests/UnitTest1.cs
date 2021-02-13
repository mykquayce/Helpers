using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Helpers.DockerHub.Tests
{
	public class UnitTest1 : IClassFixture<XUnitClassFixtures.HttpClientFixture>, IClassFixture<XUnitClassFixtures.UserSecretsFixture>
	{
		private readonly string _username, _password;
		private readonly HttpMessageInvoker _httpClient;

		public UnitTest1(
			XUnitClassFixtures.HttpClientFixture httpClientFixture,
			XUnitClassFixtures.UserSecretsFixture userSecretsFixture)
		{
			_httpClient = httpClientFixture.HttpClient;
			_password = userSecretsFixture["DockerHub:Password"];
			_username = userSecretsFixture["DockerHub:Username"];
		}

		[Theory]
		[InlineData("library", "ubuntu")]
		public async Task GetTokenTest(string organisation, string repository)
		{
			var now = DateTime.UtcNow;

			var (token, expiry) = await GetTokenAsync(organisation, repository, _username, _password);

			Assert.NotNull(token);
			Assert.NotEmpty(token);
			Assert.NotEqual(default, expiry);
			Assert.InRange(expiry, now, now.AddHours(1));
		}

		[Theory]
		[InlineData("library", "ubuntu")]
		public async Task GetTagsTest(string organisation, string repository)
		{
			var (token, _) = await GetTokenAsync(organisation, repository, _username, _password);

			var tags = GetTagsAsync(token, organisation, repository);

			var count = 0;

			await foreach (var tag in tags)
			{
				count++;
				Assert.NotNull(tag);
				Assert.NotEmpty(tag);
			}

			Assert.InRange(count, 1, int.MaxValue);
		}

		[Theory]
		[InlineData("library", "ubuntu", "latest")]
		public async Task GetManifestsTest(string organisation, string repository, string tag)
		{
			var uriString = $"https://registry-1.docker.io/v2/{organisation}/{repository}/manifests/{tag}";

			var (token, _) = await GetTokenAsync(organisation, repository, _username, _password);

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString)
			{
				Headers =
				{
					Authorization = new AuthenticationHeaderValue("Bearer", token),
				},
			};

			var responseMessage = await _httpClient.SendAsync(requestMessage, CancellationToken.None);

			var stream = await responseMessage.Content.ReadAsStreamAsync();

			var response = await JsonSerializer.DeserializeAsync<ManifestsResponseObject>(stream);

			Assert.NotNull(response);
		}

		private async Task<(string, DateTime)> GetTokenAsync(string organisation, string repository, string username, string password, CancellationToken? cancellationToken = default)
		{
			var scope = HttpUtility.UrlEncode($"repository:{organisation}/{repository}:pull");
			var uriString = $"https://auth.docker.io/token?service=registry.docker.io&scope={scope}&offline_token=1&client_id=shell";

			var credentials = $"{username}:{password}".ToBase64String();

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString)
			{
				Headers =
				{
					Authorization = new AuthenticationHeaderValue("Basic", credentials),
				},
			};

			var responseMessage = await _httpClient.SendAsync(requestMessage, cancellationToken ?? CancellationToken.None);

			var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken ?? CancellationToken.None);

			var response = await JsonSerializer.DeserializeAsync<AuthResponseObject>(stream);

			var token = response.token ?? throw new ArgumentNullException(nameof(response.token));
			var expiry = DateTime.UtcNow.AddSeconds(response.expires_in ?? 0);

			return (token, expiry);
		}

		private async IAsyncEnumerable<string> GetTagsAsync(string token, string organisation, string repository)
		{
			var uriString = $"https://registry-1.docker.io/v2/{organisation}/{repository}/tags/list";

			var requestMessage = new HttpRequestMessage(HttpMethod.Get, uriString)
			{
				Headers =
				{
					Authorization = new AuthenticationHeaderValue("Bearer", token),
				},
			};

			var responseMessage = await _httpClient.SendAsync(requestMessage, CancellationToken.None);

			var stream = await responseMessage.Content.ReadAsStreamAsync();

			var response = await JsonSerializer.DeserializeAsync<TagsResponseObject>(stream);

			if (response.tags is null) yield break;

			foreach (var tag in response.tags!)
			{
				yield return tag;
			}
		}

		[Fact]
		public async Task DeserializeManifestsTest()
		{
			var json = @"{
   ""schemaVersion"": 1,
   ""name"": ""library/ubuntu"",
   ""tag"": ""latest"",
   ""architecture"": ""amd64"",
   ""fsLayers"": [
	  {
		 ""blobSum"": ""sha256:a3ed95caeb02ffe68cdd9fd84406680ae93d633cb16422d00e8a7c22955b46d4""
	  },
	  {
		 ""blobSum"": ""sha256:4e643cc37772c094642f3168c56d1fcbcc9a07ecf72dbb5afdc35baf57e8bc29""
	  },
	  {
		 ""blobSum"": ""sha256:2821b8e766f41f4f148dc2d378c41d60f3d2cbe6f03b2585dd5653c3873740ef""
	  },
	  {
		 ""blobSum"": ""sha256:97058a342707e39028c2597a4306fd3b1a2ebaf5423f8e514428c73fa508960c""
	  },
	  {
		 ""blobSum"": ""sha256:692c352adcf2821d6988021248da6b276cb738808f69dcc7bbb74a9c952146f7""
	  }
   ],
   ""history"": [
	  {
		 ""v1Compatibility"": ""{\""architecture\"":\""amd64\"",\""config\"":{\""Hostname\"":\""\"",\""Domainname\"":\""\"",\""User\"":\""\"",\""AttachStdin\"":false,\""AttachStdout\"":false,\""AttachStderr\"":false,\""Tty\"":false,\""OpenStdin\"":false,\""StdinOnce\"":false,\""Env\"":[\""PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin\""],\""Cmd\"":[\""/bin/bash\""],\""ArgsEscaped\"":true,\""Image\"":\""sha256:8437baa15ca1576161e9e3f0981298a9c8f0c027e2f86b8d4336bb0d54c2896a\"",\""Volumes\"":null,\""WorkingDir\"":\""\"",\""Entrypoint\"":null,\""OnBuild\"":null,\""Labels\"":null},\""container\"":\""6255a9da773a5e0438e3c097b876a2de65d33f3fb57c4e515faed215d17b8b5d\"",\""container_config\"":{\""Hostname\"":\""6255a9da773a\"",\""Domainname\"":\""\"",\""User\"":\""\"",\""AttachStdin\"":false,\""AttachStdout\"":false,\""AttachStderr\"":false,\""Tty\"":false,\""OpenStdin\"":false,\""StdinOnce\"":false,\""Env\"":[\""PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin\""],\""Cmd\"":[\""/bin/sh\"",\""-c\"",\""#(nop) \"",\""CMD [\\\""/bin/bash\\\""]\""],\""ArgsEscaped\"":true,\""Image\"":\""sha256:8437baa15ca1576161e9e3f0981298a9c8f0c027e2f86b8d4336bb0d54c2896a\"",\""Volumes\"":null,\""WorkingDir\"":\""\"",\""Entrypoint\"":null,\""OnBuild\"":null,\""Labels\"":{}},\""created\"":\""2020-07-06T21:56:31.471255509Z\"",\""docker_version\"":\""18.09.7\"",\""id\"":\""7e46f8ee8124d43b7ce80dcc4a59f144a0fcccea46950e8027c2ab31f3a8f40c\"",\""os\"":\""linux\"",\""parent\"":\""be5d1f35efd015a7897ba70d66fdb5fb8c3a388176a59fc3f08e1ece98617501\"",\""throwaway\"":true}""
	  },
	  {
		 ""v1Compatibility"": ""{\""id\"":\""be5d1f35efd015a7897ba70d66fdb5fb8c3a388176a59fc3f08e1ece98617501\"",\""parent\"":\""af67310e14120aa822899b6e472e3dcaf4b0ab8dfe7cb96d25a5293dc291c23e\"",\""created\"":\""2020-07-06T21:56:31.295257919Z\"",\""container_config\"":{\""Cmd\"":[\""/bin/sh -c mkdir -p /run/systemd \\u0026\\u0026 echo 'docker' \\u003e /run/systemd/container\""]}}""
	  },
	  {
		 ""v1Compatibility"": ""{\""id\"":\""af67310e14120aa822899b6e472e3dcaf4b0ab8dfe7cb96d25a5293dc291c23e\"",\""parent\"":\""ffc8d6957147a72d81040439a66cb5b620e0a8d919976875dff72e594cff648f\"",\""created\"":\""2020-07-06T21:56:30.474974715Z\"",\""container_config\"":{\""Cmd\"":[\""/bin/sh -c set -xe \\t\\t\\u0026\\u0026 echo '#!/bin/sh' \\u003e /usr/sbin/policy-rc.d \\t\\u0026\\u0026 echo 'exit 101' \\u003e\\u003e /usr/sbin/policy-rc.d \\t\\u0026\\u0026 chmod +x /usr/sbin/policy-rc.d \\t\\t\\u0026\\u0026 dpkg-divert --local --rename --add /sbin/initctl \\t\\u0026\\u0026 cp -a /usr/sbin/policy-rc.d /sbin/initctl \\t\\u0026\\u0026 sed -i 's/^exit.*/exit 0/' /sbin/initctl \\t\\t\\u0026\\u0026 echo 'force-unsafe-io' \\u003e /etc/dpkg/dpkg.cfg.d/docker-apt-speedup \\t\\t\\u0026\\u0026 echo 'DPkg::Post-Invoke { \\\""rm -f /var/cache/apt/archives/*.deb /var/cache/apt/archives/partial/*.deb /var/cache/apt/*.bin || true\\\""; };' \\u003e /etc/apt/apt.conf.d/docker-clean \\t\\u0026\\u0026 echo 'APT::Update::Post-Invoke { \\\""rm -f /var/cache/apt/archives/*.deb /var/cache/apt/archives/partial/*.deb /var/cache/apt/*.bin || true\\\""; };' \\u003e\\u003e /etc/apt/apt.conf.d/docker-clean \\t\\u0026\\u0026 echo 'Dir::Cache::pkgcache \\\""\\\""; Dir::Cache::srcpkgcache \\\""\\\"";' \\u003e\\u003e /etc/apt/apt.conf.d/docker-clean \\t\\t\\u0026\\u0026 echo 'Acquire::Languages \\\""none\\\"";' \\u003e /etc/apt/apt.conf.d/docker-no-languages \\t\\t\\u0026\\u0026 echo 'Acquire::GzipIndexes \\\""true\\\""; Acquire::CompressionTypes::Order:: \\\""gz\\\"";' \\u003e /etc/apt/apt.conf.d/docker-gzip-indexes \\t\\t\\u0026\\u0026 echo 'Apt::AutoRemove::SuggestsImportant \\\""false\\\"";' \\u003e /etc/apt/apt.conf.d/docker-autoremove-suggests\""]}}""
	  },
	  {
		 ""v1Compatibility"": ""{\""id\"":\""ffc8d6957147a72d81040439a66cb5b620e0a8d919976875dff72e594cff648f\"",\""parent\"":\""59478d257c08ef4d6063b58b5440b30dad4553c5e06878393c6e4d940183b96e\"",\""created\"":\""2020-07-06T21:56:29.704325263Z\"",\""container_config\"":{\""Cmd\"":[\""/bin/sh -c [ -z \\\""$(apt-get indextargets)\\\"" ]\""]}}""
	  },
	  {
		 ""v1Compatibility"": ""{\""id\"":\""59478d257c08ef4d6063b58b5440b30dad4553c5e06878393c6e4d940183b96e\"",\""created\"":\""2020-07-06T21:56:28.828661061Z\"",\""container_config\"":{\""Cmd\"":[\""/bin/sh -c #(nop) ADD file:cf87af1f0e27aa6ffad26c57edca4ca55dc97861590a2d63475085a08d3b0063 in / \""]}}""
	  }
   ],
   ""signatures"": [
	  {
		 ""header"": {
			""jwk"": {
			   ""crv"": ""P-256"",
			   ""kid"": ""KUSO:TXKH:Y26S:ORGI:VOM3:CHCT:S6SI:LHVN:MVGO:RZER:PFFF:AXDH"",
			   ""kty"": ""EC"",
			   ""x"": ""rxNbeQZ20IXs0BoOtQQ54okjhYUZlRAIQN4YyGhucLI"",
			   ""y"": ""CQC-VmS4btot5ziAESC8b5A3LBQ8tl_ZtAtHYpzMzyo""
			},
			""alg"": ""ES256""
		 },
		 ""signature"": ""CzUo4ylpx68k7b1y5kAF6SjRKaGtAfQ3PFgYkgfGBmpiaHosHWjzCeeG8HX0WCujwoPAMKhNTAmp0OP2BWsE1A"",
		 ""protected"": ""eyJmb3JtYXRMZW5ndGgiOjQ5MzAsImZvcm1hdFRhaWwiOiJDbjAiLCJ0aW1lIjoiMjAyMC0wNy0xMFQyMDoyNjo1NVoifQ""
	  }
   ]
}";

			JsonSerializer.Deserialize<ManifestsResponseObject>(json);
		}
	}

	public static class StringExtensions
	{
		public static byte[] ToUtf8Bytes(this string s) => System.Text.Encoding.UTF8.GetBytes(s);
		public static string ToBase64String(this byte[] bytes) => Convert.ToBase64String(bytes, Base64FormattingOptions.None);
		public static string ToBase64String(this string s) => s.ToUtf8Bytes().ToBase64String();
	}

	public class AuthResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? token { get; set; }
		public string? access_token { get; set; }
		public int? expires_in { get; set; }
		public DateTime? issued_at { get; set; }
#pragma warning restore IDE1006 // Naming Styles
	}


	public class TagsResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public string? name { get; set; }
		public string[]? tags { get; set; }
#pragma warning restore IDE1006 // Naming Styles
	}


	public class ManifestsResponseObject
	{
		public int schemaVersion { get; set; }
		public string name { get; set; }
		public string tag { get; set; }
		public string architecture { get; set; }
		public Dictionary<string, string> fsLayers { get; set; }
		public History[] history { get; set; }
		public Signature[] signatures { get; set; }
	}

	public class Fslayer
	{
		public string blobSum { get; set; }
	}

	public class History
	{
		public string v1Compatibility { get; set; }
	}

	public class Signature
	{
		public Header header { get; set; }
		public string signature { get; set; }
		public string _protected { get; set; }
	}

	public class Header
	{
		public Dictionary<string, string> jwk { get; set; }
		public string alg { get; set; }
	}

}
