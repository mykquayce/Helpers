using Helpers.Elgato.Clients.Concrete;
using System.Text.Json;
using Xunit;

namespace Helpers.Elgato.Tests
{
	public class ElgatoClientConfigTests
	{
		[Fact]
		public void ParameterlessConstructor()
		{
			var config = new ElgatoClient.Config();

			Assert.Equal(ElgatoClient.Config.DefaultScheme, config.Scheme);
			Assert.Equal(ElgatoClient.Config.DefaultHost, config.Host);
			Assert.Equal(ElgatoClient.Config.DefaultPort, config.Port);
		}

		[Theory]
		[InlineData("rstdhinrse")]
		[InlineData("kniedsptknespdtg")]
		[InlineData("st")]
		public void SetScheme(string scheme)
		{
			var config = new ElgatoClient.Config(Scheme: scheme);

			Assert.Equal(scheme, config.Scheme);
			Assert.Equal(ElgatoClient.Config.DefaultHost, config.Host);
			Assert.Equal(ElgatoClient.Config.DefaultPort, config.Port);
		}

		[Theory]
		[InlineData("rsitoednoire")]
		[InlineData("nkeudtbckneibdtdbct")]
		[InlineData("hnktdvs")]
		public void SetHost(string host)
		{
			var config = new ElgatoClient.Config(Host: host);

			Assert.Equal(ElgatoClient.Config.DefaultScheme, config.Scheme);
			Assert.Equal(host, config.Host);
			Assert.Equal(ElgatoClient.Config.DefaultPort, config.Port);
		}

		[Theory]
		[InlineData(49_875)]
		[InlineData(29_857)]
		[InlineData(3)]
		public void SetPort(ushort port)
		{
			var config = new ElgatoClient.Config(Port: port);

			Assert.Equal(ElgatoClient.Config.DefaultScheme, config.Scheme);
			Assert.Equal(ElgatoClient.Config.DefaultHost, config.Host);
			Assert.Equal(port, config.Port);
		}

		[Theory]
		[InlineData("{}")]
		[InlineData("{ }")]
		public void DeserializeFromEmpty(string json)
		{
			var config = JsonSerializer.Deserialize<ElgatoClient.Config>(json);

			Assert.Equal(ElgatoClient.Config.DefaultScheme, config!.Scheme);
			Assert.Equal(ElgatoClient.Config.DefaultHost, config.Host);
			Assert.Equal(ElgatoClient.Config.DefaultPort, config.Port);
		}

		[Theory]
		[InlineData(@"{""Scheme"":""bvhnevbecnh""}", "bvhnevbecnh")]
		[InlineData(@"{""Scheme"":""xcvbkm""}", "xcvbkm")]
		public void DeserializeWithScheme(string json, string expected)
		{
			var config = JsonSerializer.Deserialize<ElgatoClient.Config>(json);

			Assert.Equal(expected, config!.Scheme);
			Assert.Equal(ElgatoClient.Config.DefaultHost, config.Host);
			Assert.Equal(ElgatoClient.Config.DefaultPort, config.Port);
		}

		[Theory]
		[InlineData(@"{""Host"":""nehtxksvnehkxbtvd""}", "nehtxksvnehkxbtvd")]
		[InlineData(@"{""Host"":""uynjghiguenyjh""}", "uynjghiguenyjh")]
		public void DeserializeWithHost(string json, string expected)
		{
			var config = JsonSerializer.Deserialize<ElgatoClient.Config>(json);

			Assert.Equal(ElgatoClient.Config.DefaultScheme, config!.Scheme);
			Assert.Equal(expected, config.Host);
			Assert.Equal(ElgatoClient.Config.DefaultPort, config.Port);
		}

		[Theory]
		[InlineData(@"{""Port"":43523}", 43_523)]
		[InlineData(@"{""Port"":123}", 123)]
		public void DeserializeWithPort(string json, ushort expected)
		{
			var config = JsonSerializer.Deserialize<ElgatoClient.Config>(json);

			Assert.Equal(ElgatoClient.Config.DefaultScheme, config!.Scheme);
			Assert.Equal(ElgatoClient.Config.DefaultHost, config.Host);
			Assert.Equal(expected, config.Port);
		}
	}
}
