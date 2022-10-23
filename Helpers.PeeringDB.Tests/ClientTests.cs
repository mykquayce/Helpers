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
			_client = new HttpClient(handler) { BaseAddress = new Uri("https://www.peeringdb.com/", UriKind.Absolute), };
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
		[InlineData("meta", 9_660)]
		[InlineData("netflix", 6_483)]
		public async Task SearchOrganisations(string term, params int[] expectedIds)
		{
			// Act
			var organisations = await SearchOrganisationsAsync(term).ToListAsync();

			// Assert
			Assert.NotNull(organisations);
			Assert.NotEmpty(organisations);
			Assert.All(expectedIds, id => Assert.Contains(id, organisations.Select(o => o.id)));
		}

		[Theory]
		[InlineData("meta", 32_934, 63_293)]
		[InlineData("netflix", 2_906, 40_027, 55_095)]
		public async Task SearchNetworks(string term, params int[] expectedAsns)
		{
			// Act
			var networks = await SearchNetworksAsync(term).ToListAsync();

			// Assert
			Assert.NotNull(networks);
			Assert.NotEmpty(networks);
			Assert.All(expectedAsns, asn => Assert.Contains(asn, networks.Select(n => n.asn)));
		}

		[Theory]
		[InlineData(714, "Apple Inc.")]
		[InlineData(2_906, "Netflix")]
		[InlineData(6_185, "Apple CDN")]
		[InlineData(32_934, "Meta")]
		[InlineData(40_027, "Netflix Streaming Services")]
		[InlineData(55_095, "Netflix AS55095")]
		[InlineData(63_293, "Meta AS63293")]
		public async Task SearchAsn(int asn, string expected)
		{
			var net = await GetNetworkByAsnAsync(asn);
			Assert.NotNull(net);
			Assert.Equal(expected, net!.name);
		}

		private async IAsyncEnumerable<int> GetAsnByOrganisationIdAsync(int id)
		{
			var org = await GetOrganisationByIdAsync(id);
			if (org is null) yield break;

			foreach (var netId in org.net_set)
			{
				var net = await GetNetworkByIdAsync(netId);
				if (net is not null) yield return net.asn;
			}
		}

		private ValueTask<org?> GetOrganisationByIdAsync(int id) => GetAsync<org>($"api/org/{id:D}?depth=1").FirstOrDefaultAsync();
		private IAsyncEnumerable<org> SearchOrganisationsAsync(string nameFragment) => GetAsync<org>($"api/org?name__contains={nameFragment}&depth=1");
		private ValueTask<net?> GetNetworkByIdAsync(int id) => GetAsync<net>($"api/net/{id:D}?depth=0").FirstOrDefaultAsync();
		private ValueTask<net?> GetNetworkByAsnAsync(int asn) => GetAsync<net>($"api/net?asn={asn:D}&depth=0").FirstOrDefaultAsync();

		private IAsyncEnumerable<net> SearchNetworksAsync(string nameFragment) => GetAsync<net>($"api/net?name__contains={nameFragment}&depth=0");

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
