using static Dawn.Guard;

namespace Dawn;

public static class DawnGuardExtensions
{
	public const string IdPattern = @"^t\d_[0-9a-z]{6,7}$";
	public const string SubredditNamePattern = "^[0-9A-Z_a-z]{2,}$";
	public const string ThreadIdPattern = "^[0-9a-z]{6}$";

	public static ref readonly ArgumentInfo<string> IsId(in this ArgumentInfo<string> argument)
		=> ref argument.NotNull().NotEmpty().NotWhiteSpace().Matches(IdPattern, (s, b) => s + " is not a valid ID");

	public static ref readonly ArgumentInfo<string> IsSubredditName(in this ArgumentInfo<string> argument)
		=> ref argument.NotNull().NotEmpty().NotWhiteSpace().Matches(SubredditNamePattern, (s, b) => s + " is not a valid subreddit name");

	public static ref readonly ArgumentInfo<string> IsThreadId(in this ArgumentInfo<string> argument)
		=> ref argument.NotNull().NotEmpty().NotWhiteSpace().Matches(ThreadIdPattern, (s, b) => s + " is not a valid thread ID");
}
