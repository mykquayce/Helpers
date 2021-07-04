using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helpers.Json.Converters
{
	public class JsonIPEndPointConverter : JsonConverter<IPEndPoint>
	{
		public override IPEndPoint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return TryParse(reader.GetString(), out var endPoint)
				? endPoint
				: default;
		}

		public override void Write(Utf8JsonWriter writer, IPEndPoint value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}

		public static bool TryParse(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			return s.Where(c => c == ':').Skip(1).Any()
				? TryParseIPv6(s, out endPoint)
				: TryParseIPv4(s, out endPoint);
		}

		public static bool TryParseIPv6(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			return s.Contains("]:")
				? TryParseIPv6WithPort(s, out endPoint)
				: TryParseIPv6WithoutPort(s, out endPoint);
		}

		public static bool TryParseIPv6WithPort(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			var col = s.LastIndexOf("]:");

			if (IPAddress.TryParse(s[..(col + 1)], out var ip)
				&& int.TryParse(s[(col + 2)..], out var port))
			{
				endPoint = new(ip, port);
				return true;
			}

			endPoint = default;
			return false;
		}

		public static bool TryParseIPv6WithoutPort(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			if (IPAddress.TryParse(s, out var ip))
			{
				endPoint = new(ip, IPEndPoint.MinPort);
				return true;
			}

			endPoint = default;
			return false;
		}

		public static bool TryParseIPv4WithoutPort(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			if (IPAddress.TryParse(s, out var ip))
			{
				endPoint = new(ip, IPEndPoint.MinPort);
				return true;
			}

			endPoint = default;
			return false;
		}

		public static bool TryParseIPv4WitnPort(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			var col = s.LastIndexOf(':');

			if (IPAddress.TryParse(s[..col], out var ip)
				&& int.TryParse(s[(col + 1)..], out var port))
			{
				endPoint = new(ip, port);
				return true;
			}

			endPoint = default;
			return false;
		}

		public static bool TryParseIPv4(string s, [NotNullWhen(true)] out IPEndPoint? endPoint)
		{
			return s.Contains(':')
				? TryParseIPv4WitnPort(s, out endPoint)
				: TryParseIPv4WithoutPort(s, out endPoint);
		}
	}
}
