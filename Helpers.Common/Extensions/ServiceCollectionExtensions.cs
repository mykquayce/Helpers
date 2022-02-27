using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	private const BindingFlags _propertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;

	public static IServiceCollection FileConfigure<TOptions>(this IServiceCollection services, IConfiguration configuration)
		where TOptions : class
	{
		var type = typeof(TOptions);
		var properties = type.GetProperties(_propertyFlags);

		var additionals = new Dictionary<string, string>();

		foreach (var property in from p in properties
								 where p.PropertyType == typeof(string)
								 where configuration[p.Name] is null
								 select p)
		{
			var path = configuration[property.Name + "_File"];
			if (string.IsNullOrWhiteSpace(path)) continue;
			var contents = File.ReadAllText(path.FixPaths());
			if (string.IsNullOrWhiteSpace(contents)) continue;
			additionals.Add(property.Name, contents);
		}

		var augmentedConfiguration = new ConfigurationBuilder()
			.AddConfiguration(configuration)
			.AddInMemoryCollection(additionals)
			.Build();

		services.Configure<TOptions>(augmentedConfiguration);

		return services;
	}
}
