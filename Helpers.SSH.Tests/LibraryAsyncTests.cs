﻿using Helpers.SSH.Tests.Fixtures;
using Xunit;

namespace Helpers.SSH.Tests;

public sealed class LibraryAsyncTests : IClassFixture<LibraryFixture>
{
	private readonly Renci.SshNet.SshClient _sut;

	public LibraryAsyncTests(LibraryFixture libraryFixture)
	{
		_sut = libraryFixture.Library;
	}

	[Theory]
	[InlineData("echo Hello world", "Hello world\n")]
	[InlineData("date -Iseconds", @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\+\d{2}:\d{2}$")]
	[InlineData("cat /tmp/dhcp.leases", @"(\d+ \w\w:\w\w:\w\w:\w\w:\w\w:\w\w \d+\.\d+\.\d+\.\d+ .+? .+?)+")]
	public async Task Async(string commandText, string expected)
	{
		using var command = _sut.CreateCommand(commandText);

		command.CommandTimeout = TimeSpan.FromSeconds(5);

		var task = Task.Factory.FromAsync<string>(
			beginMethod: (callback, state) => command.BeginExecute(callback, state),
			endMethod: result => command.EndExecute(result),
			state: commandText);

		var actual = await task;

		Assert.NotNull(actual);
		Assert.NotEmpty(actual);
		Assert.Matches(expected, actual);
	}
}
