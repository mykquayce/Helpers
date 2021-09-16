using Dawn;
using System;
using System.Collections.Generic;

namespace Helpers.Common
{
	public static class EnvironmentHelpers
	{
		private static readonly EnvironmentVariableTarget[] _targets = new[]
		{
			EnvironmentVariableTarget.Process,
			EnvironmentVariableTarget.User,
			EnvironmentVariableTarget.Machine,
		};

		internal static Func<string, EnvironmentVariableTarget, string?> EnvironmentVariableGetter { get; set; } = (variable, target) => Environment.GetEnvironmentVariable(variable, target);

		public static string GetEnvironmentVariable(string variable)
		{
			Guard.Argument(variable).NotNull().NotEmpty().NotWhiteSpace();

			foreach (var target in _targets)
			{
				var value = EnvironmentVariableGetter(variable, target);

				if (value != default)
				{
					return value;
				}
			}

			throw new KeyNotFoundException($"Environment variable {variable} not found")
			{
				Data = { [nameof(variable)] = variable, },
			};
		}
	}
}
