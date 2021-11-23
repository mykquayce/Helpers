using System.Text.RegularExpressions;
using static Dawn.Guard;

namespace Dawn;

public static class DawnGuardExtensions
{
	public static Regex UuidRegex { get; } = new(@"^[\-\.\/0-9A-Z_a-z]{1,128}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	public static ref readonly ArgumentInfo<string> IsTagName(in this ArgumentInfo<string> argument)
	{
		var message = $"{argument.Name} must be a valid tag name (matching {UuidRegex})";
		return ref argument.NotNull(message).NotEmpty(message).NotWhiteSpace(message).Matches(UuidRegex, (s, b) => message);
	}
}
