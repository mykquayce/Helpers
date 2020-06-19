using System;

namespace Helpers.Twitch.Models
{
	public class GetFollowingResponseObject
	{
#pragma warning disable IDE1006 // Naming Styles
		public int? total { get; set; }
		public Datum[]? data { get; set; }
		public Pagination? pagination { get; set; }

		public class Datum
		{
			public string? from_id { get; set; }
			public string? from_name { get; set; }
			public string? to_id { get; set; }
			public string? to_name { get; set; }
			public DateTime? followed_at { get; set; }
		}
#pragma warning restore IDE1006 // Naming Styles
	}
}
