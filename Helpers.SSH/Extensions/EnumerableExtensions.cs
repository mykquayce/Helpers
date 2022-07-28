namespace System.Collections.Generic;

public static class EnumerableExtensions
{
	public static void Deconstruct<T>(this IEnumerable<T> values, out T? first, out T? second)
		=> Deconstruct(values, out first, out second, out _, out _, out _, out _);
	public static void Deconstruct<T>(this IEnumerable<T> values, out T? first, out T? second, out T? third)
		=> Deconstruct(values, out first, out second, out third, out _, out _, out _);
	public static void Deconstruct<T>(this IEnumerable<T> values, out T? first, out T? second, out T? third, out T? fourth)
		=> Deconstruct(values, out first, out second, out third, out fourth, out _, out _);
	public static void Deconstruct<T>(this IEnumerable<T> values, out T? first, out T? second, out T? third, out T? fourth, out T? fifth)
		=> Deconstruct(values, out first, out second, out third, out fourth, out fifth, out _);

	public static void Deconstruct<T>(this IEnumerable<T> values, out T? first, out T? second, out T? third, out T? fourth, out T? fifth, out T? sixth)
	{
		using var enumerator = values.GetEnumerator();

		first = f();
		second = f();
		third = f();
		fourth = f();
		fifth = f();
		sixth = f();

		T? f() => enumerator!.MoveNext() ? enumerator.Current : default;
	}
}
