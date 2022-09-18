using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters;

public class JsonBigIntegerConverter : JsonDelegateConverter<BigInteger>
{
	public JsonBigIntegerConverter() : base(BigInteger.TryParse) { }
}

public class JsonBoolConverter : JsonDelegateConverter<bool>
{
	public JsonBoolConverter() : base(bool.TryParse) { }
}

public class JsonByteConverter : JsonDelegateConverter<byte>
{
	public JsonByteConverter() : base(byte.TryParse) { }
}

public class JsonCharConverter : JsonDelegateConverter<char>
{
	public JsonCharConverter() : base(char.TryParse) { }
}

public class JsonDateTimeConverter : JsonDelegateConverter<DateTime>
{
	public JsonDateTimeConverter() : base(DateTime.TryParse) { }
}

public class JsonDoubleConverter : JsonDelegateConverter<double>
{
	public JsonDoubleConverter() : base(double.TryParse) { }
}

public class JsonFloatConverter : JsonDelegateConverter<float>
{
	public JsonFloatConverter() : base(float.TryParse) { }
}

public class JsonGuidConverter : JsonDelegateConverter<Guid>
{
	public JsonGuidConverter() : base(Guid.TryParse) { }
}

public class JsonIntConverter : JsonDelegateConverter<int>
{
	public JsonIntConverter() : base(int.TryParse) { }
}

public class JsonIPAddressConverter : JsonDelegateConverter<IPAddress>
{
	public JsonIPAddressConverter() : base(IPAddress.TryParse) { }
}

public class JsonLongConverter : JsonDelegateConverter<long>
{
	public JsonLongConverter() : base(long.TryParse) { }
}

public class JsonSByteConverter : JsonDelegateConverter<sbyte>
{
	public JsonSByteConverter() : base(sbyte.TryParse) { }
}

public class JsonShortConverter : JsonDelegateConverter<short>
{
	public JsonShortConverter() : base(short.TryParse) { }
}

public class JsonTimeSpanConverter : JsonDelegateConverter<TimeSpan>
{
	public JsonTimeSpanConverter() : base(TimeSpan.TryParse) { }
}

public class JsonUIntConverter : JsonDelegateConverter<uint>
{
	public JsonUIntConverter() : base(uint.TryParse) { }
}

public class JsonULongConverter : JsonDelegateConverter<ulong>
{
	public JsonULongConverter() : base(ulong.TryParse) { }
}

public class JsonUShortConverter : JsonDelegateConverter<ushort>
{
	public JsonUShortConverter() : base(ushort.TryParse) { }
}

public class JsonDelegateConverter<T> : JsonConverter<T>
{
	private readonly JsonDelegateConverter<T>.TryParseDelegate _tryParseDelegate;

	public delegate bool TryParseDelegate(string s, [MaybeNullWhen(false)] out T value);

	public JsonDelegateConverter(TryParseDelegate tryParseDelegate)
	{
		_tryParseDelegate = tryParseDelegate;
	}

	public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var s = reader.GetString();

		return !string.IsNullOrWhiteSpace(s) && _tryParseDelegate(s, out var result)
			? result
			: default;
	}

	public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value?.ToString() ?? string.Empty);
	}
}
