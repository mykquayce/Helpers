using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Helpers.Common.Tests
{
	public class EnvironmentHelpersTests : IDisposable
	{
		private readonly Func<string, EnvironmentVariableTarget, string?> _oldEnvironmentVariableGetter;

		public EnvironmentHelpersTests()
		{
			_oldEnvironmentVariableGetter = EnvironmentHelpers.EnvironmentVariableGetter;
		}

		[Theory]
		[InlineData("process", "user", "machine", "process")]
		[InlineData(default, "user", "machine", "user")]
		[InlineData(default, default, "machine", "machine")]
		[InlineData("", "user", "machine", "")]
		public void EnvironmentHelpersTests_GetEnvironmentVariable(string? process, string? user, string? machine, string expected)
		{
			// Arrange
			ConfigureEnvironmentVariableGetter(process, user, machine);

			// Act
			var actual = EnvironmentHelpers.GetEnvironmentVariable("jlhdfprtljedhfrptaejldhfprt");

			// Assert
			Assert.Equal(expected, actual);
		}

		private void ConfigureEnvironmentVariableGetter(
			string? processEnvironmentVariable,
			string? userEnvironmentVariable,
			string? machineEnvironmentVariable)
		{
			EnvironmentHelpers.EnvironmentVariableGetter = (string _, EnvironmentVariableTarget target) => target switch
			{
				EnvironmentVariableTarget.Process => processEnvironmentVariable,
				EnvironmentVariableTarget.User => userEnvironmentVariable,
				EnvironmentVariableTarget.Machine => machineEnvironmentVariable,
				_ => throw new ArgumentOutOfRangeException(nameof(target), target, $"Unknown {nameof(Environment)}: {target}")
				{
					Data = { [nameof(target)] = target, },
				},
			};
		}

		public void Dispose()
		{
			EnvironmentHelpers.EnvironmentVariableGetter = _oldEnvironmentVariableGetter;
		}
	}
}
