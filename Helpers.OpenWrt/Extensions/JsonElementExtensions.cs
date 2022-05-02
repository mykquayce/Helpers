namespace System.Text.Json;

public static class JsonElementExtensions
{
	public static IEnumerable<string?> GetStrings(this JsonElement jsonElement)
	{
		switch (jsonElement.ValueKind)
		{
			case JsonValueKind.String:
				yield return jsonElement.GetString();
				break;
			case JsonValueKind.Array:
				foreach (var s in from a in jsonElement.EnumerateArray()
								  from s in a.GetStrings()
								  select s)
				{
					yield return s;
				}
				break;
		}
	}
}
