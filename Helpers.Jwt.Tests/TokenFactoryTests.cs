using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using Xunit;

namespace Helpers.Jwt.Tests
{
	public class TokenFactoryTests
	{
		private const string _key = "TG9yZW0gaXBzdW0gZG9sb3Igc2l0IGFtZXQsIGNvbnNlY3RldHVyIGFkaXBpc2NpbmcgZWxpdCwgc2VkIGRvIGVpdXNtb2QgdGVtcG9yIGluY2lkaWR1bnQgdXQgbGFib3JlIGV0IGRvbG9yZSBtYWduYSBhbGlxdWEu";
		private const string _issuer = "http://localhost";
		private const string _audience = "audience";

		private readonly TokenFactory _tokenFactory;

		public TokenFactoryTests()
		{
			_tokenFactory = new TokenFactory(_key, _issuer, _audience);
		}

		[Theory]
		[InlineData(Roles.Admin, default, 1_000)]
		[InlineData(Roles.User, "test", 1_000)]
		[InlineData(Roles.Admin | Roles.User, "test", 1_000)]
		public void GenerateTest(Roles roles, string? name, int iterations = 100)
		{
			while (--iterations >= 0)
			{
				var token = _tokenFactory.Generate(roles, name);

				TokenTests(token);
			}
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(100)]
		[InlineData(1_000)]
		[InlineData(10_000)]
		[InlineData(100_000)]
		public void GenerateWithExpiryTest(int expirySeconds)
		{
			var now = DateTime.UtcNow;
			var expiry = TimeSpan.FromSeconds(expirySeconds);

			var token = _tokenFactory.Generate(Roles.Admin, default, expiry);

			var parts = token.Split('.')
				.Select(WebEncoders.Base64UrlDecode)
				.Select(System.Text.Encoding.UTF8.GetString)
				.ToList();

			var payloadJson = parts[1];

			var payload = JsonSerializer.Deserialize<PayloadObject>(payloadJson);

			var actual = (payload!.ExpiryDateTime - now).TotalSeconds;

			Assert.InRange(actual, expirySeconds - 1, expirySeconds + 1);
		}

		private static void TokenTests(string token)
		{
			var now = DateTime.UtcNow;

			Assert.NotNull(token);
			Assert.NotEmpty(token);
			Assert.Matches(@"^[-0-9A-Z_a-z]+\.[-0-9A-Z_a-z]+\.[-0-9A-Z_a-z]+$", token);

			var parts = token.Split('.')
				.Select(WebEncoders.Base64UrlDecode)
				.Select(System.Text.Encoding.UTF8.GetString)
				.ToList();

			var headerJson = parts[0];
			var payloadJson = parts[1];
			var signature = parts[2];

			Assert.NotNull(headerJson);
			Assert.NotEmpty(headerJson);
			Assert.StartsWith("{", headerJson);
			Assert.NotEqual("{}", headerJson);

			var header = JsonSerializer.Deserialize<Dictionary<string, string>>(headerJson);

			Assert.NotNull(header);
			Assert.NotEmpty(header);
			Assert.Contains("alg", header!.Keys);
			Assert.Equal("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256", header["alg"]);
			Assert.Contains("typ", header.Keys);
			Assert.Equal("JWT", header["typ"]);

			Assert.NotNull(payloadJson);
			Assert.NotEmpty(payloadJson);
			Assert.StartsWith("{", payloadJson);
			Assert.NotEqual("{}", payloadJson);

			var payload = JsonSerializer.Deserialize<PayloadObject>(payloadJson);

			Assert.NotNull(payload);
			Assert.NotEmpty(payload);
			Assert.InRange(payload!.ExpiryDateTime, now, now.AddDays(1));
			Assert.Equal(_issuer, payload.Issuer);
			Assert.Equal(_audience, payload.Audience);
			Assert.NotEqual(default, payload.Roles);

			Assert.NotNull(signature);
			Assert.NotEmpty(signature);
		}

		public class PayloadObject : Dictionary<string, JsonElement>
		{
			private const string _expiryKey = "exp",
				_audienceKey = "aud",
				_issuerKey = "iss",
				_rolesKey = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
				_nameKey = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

			private string GetString(string key) => this.TryGetValue(key, out var e)
				? e.GetString() ?? throw new ArgumentNullException(key)
				: throw new KeyNotFoundException($"{key} {nameof(key)} not found in " + string.Join(',', this.Keys));

			public double Expiry => this.TryGetValue(_expiryKey, out var e) && e.ValueKind == JsonValueKind.Number
				? e.TryGetDouble(out var d)
					? d
					: throw new ArgumentOutOfRangeException(_expiryKey, e.GetString(), $"Unparsable {_expiryKey}: {e} ")
				: throw new KeyNotFoundException(_expiryKey);

			public DateTime ExpiryDateTime => DateTime.UnixEpoch.AddSeconds(this["exp"].GetDouble());

			public string Audience => GetString(_audienceKey);
			public string Issuer => GetString(_issuerKey);

			public Roles Roles => this.TryGetValue(_rolesKey, out var element)
				? element.GetEnum<Roles>()
				: throw new KeyNotFoundException(_rolesKey);

			public string? Name => this.TryGetValue(_nameKey, out var element)
				? element.GetString()
				: default;
		}
	}

	public static class JsonElementExtensions
	{
		public static T GetEnum<T>(this JsonElement element)
			where T : struct, Enum, IConvertible
		{
			return element.ValueKind switch
			{
				JsonValueKind.String => fromstring(element.GetString()),
				JsonValueKind.Array => fromarray(element.EnumerateArray()),
				_ => throw new ArgumentOutOfRangeException(nameof(element), element, $"Unexpected {nameof(element.ValueKind)}: {element.ValueKind}."),
			};

			static T fromstring(string? s)
			{
				ArgumentException.ThrowIfNullOrEmpty(s);

				if (Enum.TryParse<T>(s, out var roles))
				{
					return roles;
				}

				throw new ArgumentOutOfRangeException(nameof(s), s, $"Unexpected role: {s}");
			}

			static T fromarray(JsonElement.ArrayEnumerator array)
			{
				var roles = 0L;

				foreach (var role in from e in array
										let t = e.GetEnum<T>()
										let i = t.ToInt64(System.Globalization.CultureInfo.InvariantCulture)
										select i)
				{
					roles |= role;
				}

				return Enum.Parse<T>(roles.ToString("D"));
			}
		}
	}
}
