namespace Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
	public static IConfigurationBuilder ResolveFileReferences(this IConfigurationBuilder builder)
	{
		IConfiguration configuration = builder.Build();

		var additionals = configuration
			.GetFileReferences()
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		builder
			.AddInMemoryCollection(additionals);

		return builder;
	}

	private static readonly IReadOnlyCollection<string> _suffices = new string[2] { "_file", "_path", };

	public static IEnumerable<KeyValuePair<string, string?>> GetFileReferences(this IConfiguration configuration)
	{
		foreach (var (key, value) in configuration.AsEnumerable())
		{
			if (string.IsNullOrWhiteSpace(key)
				|| key.Length <= 6
				|| _suffices.All(s => !key.EndsWith(s, StringComparison.OrdinalIgnoreCase))
				|| string.IsNullOrWhiteSpace(value))
			{
				continue;
			}

			var realKey = key[..^5];

			if (configuration[realKey] is not null)
			{
				continue;
			}

			string? realValue = File.Exists(value) ? File.ReadAllText(value).TrimEnd() : null;

			yield return new(realKey, realValue);
		}
	}
}
