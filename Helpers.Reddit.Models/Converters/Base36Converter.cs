using System.Numerics;

namespace Helpers.Reddit.Models.Converters;

public static class Base36Converter
{
	private static readonly BaseConverter _converter = new("0123456789abcdefghijklmnopqrstuvwxyz");

	public static T FromString<T>(string s)
		where T : INumber<T>
		=> _converter.FromString<T>(s);

	public static string ToString<T>(T value)
		where T : INumber<T>
		=> _converter.ToString(value);
}
