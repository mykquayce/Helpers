namespace System.Text.Json;

public static class JsonExtensions
{
	public static string Serialize(this object o)
		=> JsonSerializer.Serialize(o);

	public static T Deserialize<T>(this string s)
	{
		return JsonSerializer.Deserialize<T>(s)
			?? throw new Exception($"Failed to deserialize {s} to {typeof(T).Name}")
			{
				Data =
				{
					[nameof(s)] = s,
					["type"] = typeof(T),
				},
			};
	}
}
