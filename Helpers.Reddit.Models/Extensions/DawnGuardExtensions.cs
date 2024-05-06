using System.Text.RegularExpressions;
using static Dawn.Guard;

namespace Dawn;

public static partial class DawnGuardExtensions
{
	public static ref readonly ArgumentInfo<string> IsId(in this ArgumentInfo<string> argument)
		=> ref argument.NotNull().NotEmpty().NotWhiteSpace().Matches(IdRegex(), (s, b) => s + " is not a valid ID");

	public static ref readonly ArgumentInfo<string> IsSubredditName(in this ArgumentInfo<string> argument)
		=> ref argument.NotNull().NotEmpty().NotWhiteSpace().Matches(SubredditNameRegex(), (s, b) => s + " is not a valid subreddit name");

	[GeneratedRegex(@"^t\d_[0-9a-z]{6,7}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
	private static partial Regex IdRegex();

	[GeneratedRegex(@"^[0-9A-Z_a-z]{2,}$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.NonBacktracking)]
	private static partial Regex SubredditNameRegex();
}
