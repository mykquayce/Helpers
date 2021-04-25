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
		public async Task GetAsnByOrganisationId(int orgId, params int[] expectedAsns)
		{
			var actual = await GetAsnByOrganisationIdAsync(orgId).ToListAsync();
			Assert.Equal(expectedAsns, actual);
		}

		[Theory]
		[InlineData("facebook", 9_660)]
		[InlineData("netflix", 6_483)]
		public async Task SearchOrganisations(string term, params int[] expectedIds)
		{
			// Act
			var organisations = await SearchOrganisationsAsync(term).ToListAsync();

			// Assert
			Assert.NotNull(organisations);
			Assert.NotEmpty(organisations);
			Assert.Equal(expectedIds, organisations.Select(org => org.id));
		}

		[Theory]
		[InlineData("facebook", 32_934, 63_293)]
		[InlineData("netflix", 2_906, 40_027, 55_095)]
		public async Task SearchNetworks(string term, params int[] expectedAsns)
		{
			// Act
			var networks = await SearchNetworksAsync(term).ToListAsync();

			// Assert
			Assert.NotNull(networks);
			Assert.NotEmpty(networks);
			Assert.Equal(expectedAsns, networks.Select(net => net.asn));
		}

		private async IAsyncEnumerable<int> GetAsnByOrganisationIdAsync(int id)
		{
			var org = await GetOrganisationAsync(id);
			if (org is null) yield break;

			foreach (var netId in org.net_set)
			{
				var net = await GetNetworkAsync(netId);
				if (net is not null) yield return net.asn;
			}
		}

		private ValueTask<org?> GetOrganisationAsync(int id) => GetAsync<org>($"/api/org/{id:D}?depth=1").FirstOrDefaultAsync();
		private IAsyncEnumerable<org> SearchOrganisationsAsync(string name) => GetAsync<org>($"/api/org?name__contains={name}&depth=1");
		private ValueTask<net?> GetNetworkAsync(int id) => GetAsync<net>($"/api/net/{id:D}?depth=0").FirstOrDefaultAsync();
		private IAsyncEnumerable<net> SearchNetworksAsync(string name) => GetAsync<net>($"/api/net?name__contains={name}&depth=0");

		private async IAsyncEnumerable<T> GetAsync<T>(string uri)
		{
			await using var stream = await _client.GetStreamAsync(uri);
			var response = await JsonSerializer.DeserializeAsync<Wrapper<T>>(stream);
			foreach (var t in response!.data) yield return t;
		}
	}

#pragma warning disable IDE1006 // Naming Styles
	public record Wrapper<T>(IList<T> data);
	public record org(int id, string name, IList<int> net_set, DateTime created, DateTime updated, string status)
		: @base(id, name, created, updated, status);
	public record net(int id, string name, int asn, DateTime created, DateTime updated, string status)
		: @base(id, name, created, updated, status);
	public abstract record @base(int id, string name, DateTime created, DateTime updated, string status);
#pragma warning restore IDE1006 // Naming Styles
}
