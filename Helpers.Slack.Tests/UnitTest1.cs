using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Helpers.Slack.Tests
{
	public class UnitTest1 : IClassFixture<Helpers.XUnitClassFixtures.UserSecretsFixture>
	{
		private static readonly JsonSerializerOptions _options = new()
		{
			PropertyNameCaseInsensitive = true,
		};

		private const string _slackTokenKey = "Slack:Token";

		private readonly string _token;

		public UnitTest1(Helpers.XUnitClassFixtures.UserSecretsFixture fixture)
		{
			_token = fixture[_slackTokenKey];
		}

		[Theory]
		[InlineData(@"{""ok"":true,""channels"":[{""id"":""C500H9J6N"",""name"":""general"",""is_channel"":true,""is_group"":false,""is_im"":false,""created"":1492424427,""is_archived"":false,""is_general"":true,""unlinked"":0,""name_normalized"":""general"",""is_shared"":false,""parent_conversation"":null,""creator"":""U500H9G66"",""is_ext_shared"":false,""is_org_shared"":false,""shared_team_ids"":[""T4ZU0H1PV""],""pending_shared"":[],""pending_connected_team_ids"":[],""is_pending_ext_shared"":false,""is_member"":false,""is_private"":false,""is_mpim"":false,""topic"":{""value"":""Company-wide announcements and work-based matters"",""creator"":"""",""last_set"":0},""purpose"":{""value"":""This channel is for team-wide communication and announcements. All team members are in this channel."",""creator"":"""",""last_set"":0},""previous_names"":[],""num_members"":1},{""id"":""C50LR22LF"",""name"":""random"",""is_channel"":true,""is_group"":false,""is_im"":false,""created"":1492424427,""is_archived"":false,""is_general"":false,""unlinked"":0,""name_normalized"":""random"",""is_shared"":false,""parent_conversation"":null,""creator"":""U500H9G66"",""is_ext_shared"":false,""is_org_shared"":false,""shared_team_ids"":[""T4ZU0H1PV""],""pending_shared"":[],""pending_connected_team_ids"":[],""is_pending_ext_shared"":false,""is_member"":false,""is_private"":false,""is_mpim"":false,""topic"":{""value"":""Non-work banter and water cooler conversation"",""creator"":"""",""last_set"":0},""purpose"":{""value"":""A place for non-work-related flimflam, faffing, hodge-podge or jibber-jabber you'd prefer to keep out of more focused work-related channels."",""creator"":"""",""last_set"":0},""previous_names"":[],""num_members"":1}],""response_metadata"":{""next_cursor"":""""}}")]
		public void DeserializeChannelsResponse(string json)
		{
			var response = JsonSerializer.Deserialize<Models.ChannelsResponse>(json, _options);

			Assert.NotNull(response);
			Assert.NotNull(response!.Channels);
			Assert.NotEmpty(response.Channels);

			foreach (var channel in response.Channels!)
			{
				Assert.False(string.IsNullOrWhiteSpace(channel.Id));
				Assert.False(string.IsNullOrWhiteSpace(channel.Name));
			}
		}

		[Fact]
		public async Task GetChannels()
		{
			var settings = new Models.Settings { Token = _token, };

			var client = new SlackClient(Options.Create(settings));

			var channels = client.GetChannelsAsync();

			await foreach (var channel in channels)
			{
				Assert.False(string.IsNullOrWhiteSpace(channel.Id));
				Assert.False(string.IsNullOrWhiteSpace(channel.Name));
			}
		}
	}
}
