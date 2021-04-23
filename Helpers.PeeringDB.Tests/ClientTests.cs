using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.PeeringDB.Tests
{
	public sealed class ClientTests : IDisposable
	{
		private readonly HttpClient _client;

		public ClientTests()
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			_client = new HttpClient(handler) { BaseAddress = new Uri("https://peeringdb.com", UriKind.Absolute), };
		}

		public void Dispose() => _client.Dispose();

		[Theory]
		[InlineData(6_483, 2_906, 40_027, 55_095)]
		public async Task Organisation(int id, params int[] expected)
		{
			var actual = await GetAsnByOrganisationIdAsync(id).ToListAsync();
			Assert.Equal(expected, actual);
		}

		private async IAsyncEnumerable<org> GetOrganisationsAsync(int id)
		{
			await using var stream = await _client.GetStreamAsync($"/api/org/{id:D}?depth=2");
			var response = await JsonSerializer.DeserializeAsync<OrganisationResponse>(stream);
			foreach (var org in response!.data) yield return org;
		}

		private async IAsyncEnumerable<int> GetAsnByOrganisationIdAsync(int id)
		{
			await foreach (var org in GetOrganisationsAsync(id))
			{
				foreach (var net in org.net_set)
				{
					yield return net.asn;
				}
			}
		}

		private async IAsyncEnumerable<net> GetNetworksAsync(int id)
		{
			var uri = new Uri($"/api/net/{id:D}?depth=0", UriKind.Relative);
			await using var stream = await _client.GetStreamAsync(uri);
			var response = await JsonSerializer.DeserializeAsync<NetworkResponse>(stream);
			foreach (var net in response!.data) yield return net;
		}
	}


#pragma warning disable IDE1006 // Naming Styles
	public record OrganisationResponse(IList<org> data);
	public record NetworkResponse(IList<net> data);
	public record org(int id, string name, IList<net> net_set, DateTime created, DateTime updated, string status);
	public record net(int id, string name, int asn, DateTime created, DateTime updated, string status);
#pragma warning restore IDE1006 // Naming Styles
}
