using Dawn;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DawnGuardExtensionMethods
	{
		private readonly static IReadOnlyList<char> _invalidFileNameChars = Path.GetInvalidFileNameChars();
		private readonly static IReadOnlyList<char> _validConfigKeyChars = "-0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz".ToCharArray();

		public static ref readonly Guard.ArgumentInfo<string> ValidFileName(
			in this Guard.ArgumentInfo<string> argument)
		{
			argument
				.NotNull()
				.NotEmpty()
				.NotWhiteSpace()
				.DoesNotContain(" ")
				.Require(s => s.All(c => _invalidFileNameChars.Contains(c) == false), s => $"{s} must not consist of {_invalidFileNameChars}");

			return ref argument;
		}

		public static ref readonly Guard.ArgumentInfo<string?> ValidConfigKey(
			in this Guard.ArgumentInfo<string?> argument)
		{
			argument
				.NotEmpty()
				.NotWhiteSpace()
				.DoesNotContain(" ")
				.Require(s => s.All(c => _validConfigKeyChars.Contains(c)), s => $"{s} must consist of {_validConfigKeyChars}");

			return ref argument;
		}
	}
}
