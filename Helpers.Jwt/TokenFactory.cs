using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Helpers.Jwt;

public class TokenFactory
{
	private static readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(5);

	private readonly JwtHeader _header;
	private readonly string _issuer;
	private readonly string _audience;

	public TokenFactory(string key, string issuer, string audience)
	{
		var encodedKey = WebEncoders.Base64UrlDecode(key);
		var securityKey = new SymmetricSecurityKey(encodedKey);
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

		_audience = audience;
		_header = new JwtHeader(credentials);
		_issuer = issuer;
	}

	public string Generate(Roles roles = Roles.Admin, string? name = default, TimeSpan? expiry = default)
	{
		var expirySeconds = (int)DateTime.UtcNow
			.Add(expiry ?? _defaultExpiry)
			.Subtract(DateTime.UnixEpoch)
			.TotalSeconds;

		var payload = new JwtPayload
		{
			["exp"] = expirySeconds,
			["iss"] = _issuer,
			["aud"] = _audience,
		};

		payload.AddClaims(from r in roles.Expand()
						  let s = r.ToString("G")
						  let c = new Claim(ClaimTypes.Role, s)
						  select c);

		if (!string.IsNullOrWhiteSpace(name))
		{
			var claim = new Claim(ClaimTypes.NameIdentifier, name);
			payload.AddClaim(claim);
		}

		var secToken = new JwtSecurityToken(_header, payload);
		var handler = new JwtSecurityTokenHandler();

		return handler.WriteToken(secToken);
	}
}
