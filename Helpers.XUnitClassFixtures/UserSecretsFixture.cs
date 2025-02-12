﻿using Microsoft.Extensions.Configuration;

namespace Helpers.XUnitClassFixtures;

public class UserSecretsFixture
{
	public UserSecretsFixture()
	{
		Configuration = new ConfigurationBuilder()
			.AddUserSecrets<UserSecretsFixture>()
			.Build();
	}

	public string this[string key] => Configuration[key] ?? throw new KeyNotFoundException($"{key} {nameof(key)} not found in {string.Join(',', Keys)}");
	public IEnumerable<string> Keys => Configuration.GetChildren().Select(section => section.Key);
	public IConfiguration Configuration { get; }
	public T GetSection<T>(string section) => Configuration.GetSection(section).Get<T>()
		?? throw new KeyNotFoundException($"{section} {nameof(section)} not found in {string.Join(',', Keys)}");
}
