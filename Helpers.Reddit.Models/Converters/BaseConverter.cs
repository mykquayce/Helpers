using System.Numerics;

namespace Helpers.Reddit.Models.Converters;

public class BaseConverter
{
	private readonly string _chars;

	public BaseConverter(string chars)
	{
		_chars = chars;
	}

	public T FromString<T>(string s)
		where T : INumber<T>
	{
		T @base = T.CreateChecked(_chars.Length);
		T total = T.Zero, multiplier = T.One;

		var indices = s.GetIndices(_chars).Reverse();

		foreach (var index in indices)
		{
			total += multiplier * T.CreateChecked(index);
			multiplier *= @base;
		}

		return total;
	}

	public string ToString<T>(T value)
		where T : INumber<T>
		=> new(ToChars(value).ToArray());

	public IEnumerable<char> ToChars<T>(T value)
		where T : INumber<T>
	{
		var powers = value.GetPowers(_chars.Length).Reverse();

		foreach (var power in powers)
		{
			int index = int.CreateChecked(power);
			yield return _chars[index];
		}
	}
}
