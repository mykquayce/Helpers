namespace System.Text.RegularExpressions;

public static class RegexExtensions
{
	public static void Deconstruct(this Match match, out string? first, out string? second)
		=> match.Deconstruct(out first, out second, out _, out _, out _, out _, out _);

	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third)
		=> match.Deconstruct(out first, out second, out third, out _, out _, out _, out _);

	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth)
		=> match.Deconstruct(out first, out second, out third, out fourth, out _, out _, out _);

	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth, out string? fifth)
		=> match.Deconstruct(out first, out second, out third, out fourth, out fifth, out _, out _);

	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth, out string? fifth, out string? sixth)
		=> match.Deconstruct(out first, out second, out third, out fourth, out fifth, out sixth, out _);

	public static void Deconstruct(this Match match, out string? first, out string? second, out string? third, out string? fourth, out string? fifth, out string? sixth, out string? seventh)
	{
		var enumerator = match.Groups.GetEnumerator();
		first = f();
		second = f();
		third = f();
		fourth = f();
		fifth = f();
		sixth = f();
		seventh = f();

		string? f() => enumerator!.MoveNext() && enumerator.Current is Group group ? group.Value : default;
	}

	public static IDictionary<string, string?> ToDictionary(this IEnumerable<Match> matches, IEqualityComparer<string>? comparer = default)
	{
		var dictionary = new Dictionary<string, string?>(comparer ?? StringComparer.CurrentCulture);

		foreach (var match in matches)
		{
			(_, var key, var value) = match;
			dictionary.TryAdd(key!, value);
		}

		return dictionary;
	}
}
