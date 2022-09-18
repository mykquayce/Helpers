using System.Text.Json.Serialization;

namespace System.Text.Json
{
	public static class JsonExtensions
	{
		private const JsonIgnoreCondition _ignoreCondition = JsonIgnoreCondition.WhenWritingNull;
		private static readonly JsonSerializerOptions _options = new() { DefaultIgnoreCondition = _ignoreCondition, };

		public static string Serialize(this object o)
			=> JsonSerializer.Serialize(o, _options);

		public static T Deserialize<T>(this string json)
			=> JsonSerializer.Deserialize<T>(json, _options)
			?? throw new ArgumentOutOfRangeException(nameof(json), json, message: $"could not deserialize {json} to {typeof(T).FullName}");
	}
}
