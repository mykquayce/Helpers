﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Nanoleaf.Tests;

public class DependencyInjectionTests
{
	[Fact]
	public void VariablesTests()
	{
		using var provider = new ServiceCollection()
			.AddNanoleaf(Constants.BaseAddress, Constants.Token)
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}

	[Fact]
	public void ConfigTests()
	{
		var config = new Config(Constants.BaseAddress, Constants.Token);

		using var provider = new ServiceCollection()
			.AddNanoleaf(config)
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}

	[Fact]
	public void ConfigurationTests()
	{
		var initialData = new Dictionary<string, string?>
		{
			[nameof(Constants.BaseAddress)] = Constants.BaseAddress.OriginalString,
			[nameof(Constants.Token)] = Constants.Token,
		};

		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(initialData)
			.Build();

		using var provider = new ServiceCollection()
			.AddNanoleaf(configuration)
			.BuildServiceProvider();

		Assert.NotNull(provider.GetService<IClient>());
	}
}
