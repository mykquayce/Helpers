using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.TPLink.Tests
{
	public sealed class TPLinkClientTests : IDisposable
	{
		private readonly Helpers.TPLink.ITPLinkWebClient _sut;
		private readonly string _userName, _password;

		public TPLinkClientTests()
		{
			var config = new ConfigurationBuilder()
			.AddUserSecrets("8391cb70-d94f-4863-b7e4-5659af167bc6")
			.Build();

			_userName = config["TPLink:UserName"] ?? throw new ArgumentNullException("TPLink:UserName");
			_password = config["TPLink:Password"] ?? throw new ArgumentNullException("TPLink:Password");

			_sut = new Helpers.TPLink.Concrete.TPLinkWebClient();
		}

		public void Dispose() => _sut?.Dispose();

		[Theory]
		[InlineData("HS110(UK)", "amp")]
		public async Task EndToEnd(string deviceModel, string alias)
		{
			var token = await _sut.LoginAsync(_userName, _password);

			Assert.NotNull(token);
			Assert.NotEmpty(token);
			Assert.Matches("^6bdd39aa-[0-9A-Za-z]{23}$", token);

			var devices = await _sut.GetDevicesAsync(token).ToListAsync();

			Assert.NotNull(devices);
			Assert.NotEmpty(devices);

			var device = (from d in devices
						  where string.Equals(d.deviceModel, deviceModel, StringComparison.InvariantCultureIgnoreCase)
						  where string.Equals(d.alias, alias, StringComparison.InvariantCultureIgnoreCase)
						  select d
						 ).SingleOrDefault();

			Assert.NotNull(device);
			Assert.NotNull(device!.deviceId);
			Assert.NotEmpty(device.deviceId);
			Assert.Matches("^[0-9A-Z]{40}$", device.deviceId);

			var data = await _sut.GetRealtimeDataAsync(token, device.deviceId!);

			Assert.NotNull(data);
			Assert.NotNull(data.power_mw);
			Assert.InRange(data.power_mw!.Value, 1, int.MaxValue);
		}

		[Theory]
		[InlineData("amp", "1.1.0 Build 201016 Rel.175140")]
		public async Task HasPlugUpdated(string alias, string expectedFwVer)
		{
			var token = await _sut.LoginAsync(_userName, _password);
			var device = await _sut.GetDevicesAsync(token).SingleOrDefaultAsync(d => d.alias == alias);

			Assert.NotNull(device);
			Assert.NotEqual(device!.fwVer, expectedFwVer, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
