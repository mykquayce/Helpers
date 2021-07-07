using System.Text.RegularExpressions;
using static Dawn.Guard;

namespace Dawn
{
	public static class DawnGuardExtensions
	{
		private static readonly Regex _uuidRegex = new("^GlobalCache_[0-9A-F]{12}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static ref readonly ArgumentInfo<string> IsGlobalCacheUuid(in this ArgumentInfo<string> argument)
		{
			var message = $"{argument.Name} must be a valid UUID (matching {_uuidRegex})";
			return ref argument.NotNull().NotEmpty().NotWhiteSpace().Matches(_uuidRegex, (s, b) => message);
		}
	}
}
