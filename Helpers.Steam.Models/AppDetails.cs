using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Helpers.Steam.Models
{
	public class AppDetails
	{
		[JsonPropertyName("type")]
		public string? Type { get; set; }
		[JsonPropertyName("categories")]
		public Category[]? Categories_Array { get; set; }

		[JsonIgnore]
		public IEnumerable<Models.Category> Categories
			=> from c in Categories_Array ?? Enumerable.Empty<Category>()
			   where c.Id.HasValue
			   select (Models.Category)c.Id!.Value;

		public class Category
		{
			[JsonPropertyName("id")]
			public int? Id { get; set; }
			[JsonPropertyName("description")]
			public string? Description { get; set; }
		}
	}
}
