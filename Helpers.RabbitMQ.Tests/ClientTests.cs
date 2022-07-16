using System.Net;
using System.Text.Json;
using Xunit;

namespace Helpers.RabbitMQ.Tests;

[Collection(nameof(CollectionDefinitions.NonParallelCollectionDefinitionClass))]
public class ClientTests : IClassFixture<Fixtures.ServiceProviderFixture>
{
	private readonly IClient _client;

	public ClientTests(Fixtures.ServiceProviderFixture fixture)
	{
		_client = fixture.Client;
	}

	[Theory]
	[InlineData(
		"https://linkvisitor:15671/",
		"api/queues",
		"BqMqSL1FqoHLyiBWDowTvxyR63Q5lXKKu1FHgVuna7rMkd34xFK825VedKLQPaWuSurEq2mHmk8peZXzqhbdfqApeDJ3kYYvF2LwN7Mw5gRPiKZDdHwb4sYCUXDtG85K",
		"YmYrvlR8SxAxHPCGDc3S3NtIdU9qWaNJfDkQBXBVHerJeAoxvoPAzaVy52bRE56gKlKO5sN9da6BwhBkLKrmaMkmk4mzgIjfm7II7LWFF1lPpBZaPjqHAeIUwe6lbgRr")]
	public async Task GetQueuesTests_RequestMessage(string baseAddress, string requestUri, string username, string password)
	{
		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var httpClient = new HttpClient(handler) { BaseAddress = new Uri(baseAddress), };

		using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri)
		{
			Headers =
			{
				Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",  ConvertToBase64(username, password)),
			},
		};

		using var responseMessage = await httpClient.SendAsync(requestMessage);

		Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
	}

	[Theory]
	[InlineData(
		"https://linkvisitor:15671/",
		"api/queues",
		"BqMqSL1FqoHLyiBWDowTvxyR63Q5lXKKu1FHgVuna7rMkd34xFK825VedKLQPaWuSurEq2mHmk8peZXzqhbdfqApeDJ3kYYvF2LwN7Mw5gRPiKZDdHwb4sYCUXDtG85K",
		"YmYrvlR8SxAxHPCGDc3S3NtIdU9qWaNJfDkQBXBVHerJeAoxvoPAzaVy52bRE56gKlKO5sN9da6BwhBkLKrmaMkmk4mzgIjfm7II7LWFF1lPpBZaPjqHAeIUwe6lbgRr")]
	public async Task GetQueuesTests_HttpClient(string baseAddress, string requestUri, string username, string password)
	{
		using var handler = new HttpClientHandler { AllowAutoRedirect = false, };
		using var httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri(baseAddress),
			DefaultRequestHeaders =
			{
				Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",  ConvertToBase64(username, password)),
			}
		};

		using var responseMessage = await httpClient.GetAsync(requestUri);

		Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
	}

	private static string ConvertToBase64(params string[] values)
		=> Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Join(':', values)));

	[Theory]
	[InlineData(
		"QnFNcVNMMUZxb0hMeWlCV0Rvd1R2eHlSNjNRNWxYS0t1MUZIZ1Z1bmE3ck1rZDM0eEZLODI1VmVkS0xRUGFXdVN1ckVxMm1IbWs4cGVaWHpxaGJkZnFBcGVESjNrWVl2RjJMd043TXc1Z1JQaUtaRGRId2I0c1lDVVhEdEc4NUs6WW1ZcnZsUjhTeEF4SFBDR0RjM1MzTnRJZFU5cVdhTkpmRGtRQlhCVkhlckplQW94dm9QQXphVnk1MmJSRTU2Z0tsS081c045ZGE2QndoQmtMS3JtYU1rbWs0bXpnSWpmbTdJSTdMV0ZGMWxQcEJaYVBqcUhBZUlVd2U2bGJnUnI=",
		"BqMqSL1FqoHLyiBWDowTvxyR63Q5lXKKu1FHgVuna7rMkd34xFK825VedKLQPaWuSurEq2mHmk8peZXzqhbdfqApeDJ3kYYvF2LwN7Mw5gRPiKZDdHwb4sYCUXDtG85K",
		"YmYrvlR8SxAxHPCGDc3S3NtIdU9qWaNJfDkQBXBVHerJeAoxvoPAzaVy52bRE56gKlKO5sN9da6BwhBkLKrmaMkmk4mzgIjfm7II7LWFF1lPpBZaPjqHAeIUwe6lbgRr")]
	public void Base64EncodeTests(string expected, params string[] values)
	{
		var actual = ConvertToBase64(values);
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(
		"""[{"arguments":{},"auto_delete":false,"backing_queue_status":{"avg_ack_egress_rate":0.0,"avg_ack_ingress_rate":0.0,"avg_egress_rate":0.0,"avg_ingress_rate":0.0,"delta":["delta","undefined",0,0,"undefined"],"len":0,"mode":"default","next_deliver_seq_id":0,"next_seq_id":0,"q1":0,"q2":0,"q3":0,"q4":0,"target_ram_count":"infinity","version":1},"consumer_capacity":0,"consumer_utilisation":0,"consumers":0,"durable":false,"effective_policy_definition":{},"exclusive":false,"exclusive_consumer_tag":null,"garbage_collection":{"fullsweep_after":65535,"max_heap_size":0,"min_bin_vheap_size":46422,"min_heap_size":233,"minor_gcs":48},"head_message_timestamp":null,"idle_since":"2022-07-11T14:37:31.026+00:00","memory":11832,"message_bytes":0,"message_bytes_paged_out":0,"message_bytes_persistent":0,"message_bytes_ram":0,"message_bytes_ready":0,"message_bytes_unacknowledged":0,"messages":0,"messages_details":{"rate":0.0},"messages_paged_out":0,"messages_persistent":0,"messages_ram":0,"messages_ready":0,"messages_ready_details":{"rate":0.0},"messages_ready_ram":0,"messages_unacknowledged":0,"messages_unacknowledged_details":{"rate":0.0},"messages_unacknowledged_ram":0,"name":"queue","node":"rabbit@501fd71e5a5b","operator_policy":null,"policy":null,"recoverable_slaves":null,"reductions":52809,"reductions_details":{"rate":0.0},"single_active_consumer_tag":null,"state":"running","type":"classic","vhost":"/"}]""",
		false, 0, false, 0, "queue", "rabbit@501fd71e5a5b", "running", "/")]
	public void QueuesJsonDeserializationTests(
		string json,
		bool auto_delete, int consumers, bool durable, int messages, string name, string node, string state, string vhost)
	{
		var queues = JsonSerializer.Deserialize<Models.QueueObject[]>(json);

		Assert.NotNull(queues);
		Assert.Single(queues);
		Assert.DoesNotContain(default, queues);
		Assert.Equal(auto_delete, queues[0].auto_delete);
		Assert.Equal(consumers, queues[0].consumers);
		Assert.Equal(durable, queues[0].durable);
		Assert.Equal(messages, queues[0].messages);
		Assert.Equal(name, queues[0].name);
		Assert.Equal(node, queues[0].node);
		Assert.Equal(state, queues[0].state);
		Assert.Equal(vhost, queues[0].vhost);
	}

	[Theory]
	[InlineData(
		"https://linkvisitor:15671/",
		"BqMqSL1FqoHLyiBWDowTvxyR63Q5lXKKu1FHgVuna7rMkd34xFK825VedKLQPaWuSurEq2mHmk8peZXzqhbdfqApeDJ3kYYvF2LwN7Mw5gRPiKZDdHwb4sYCUXDtG85K",
		"YmYrvlR8SxAxHPCGDc3S3NtIdU9qWaNJfDkQBXBVHerJeAoxvoPAzaVy52bRE56gKlKO5sN9da6BwhBkLKrmaMkmk4mzgIjfm7II7LWFF1lPpBZaPjqHAeIUwe6lbgRr")]
	public async Task RabbitMQRestClientTests(string baseAddress, string username, string password)
	{
		IClient sut;
		{
			var handler = new HttpClientHandler { AllowAutoRedirect = false, };
			var httpClient = new HttpClient(handler)
			{
				BaseAddress = new Uri(baseAddress),
				DefaultRequestHeaders =
				{
					Authorization =new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",  ConvertToBase64(username, password)),
				},
			};

			sut = new Concrete.RabbitMQRestClient(httpClient);
		}

		var actual = await sut.GetQueuesAsync().ToListAsync();

		Assert.Single(actual);
		Assert.DoesNotContain(default, actual);
	}

	[Theory]
	[InlineData("https", "localhost", 12_341, "https://localhost:12341/")]
	[InlineData("https:", "localhost", 12_341, "https://localhost:12341/")]
	[InlineData("https:/", "localhost", 12_341, "https://localhost:12341/")]
	[InlineData("https://", "localhost", 12_341, "https://localhost:12341/")]
	public void UriBuilderTests(string scheme, string host, int portNumber, string expected)
	{
		var builder = new UriBuilder(scheme, host, portNumber);
		var actual = builder.Uri.ToString();
		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task DependencyInjectionTests()
	{
		var queues = await _client.GetQueuesAsync().ToListAsync();

		Assert.NotEmpty(queues);
	}
}
