using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace System;

public static class ServiceProviderExtensions
{
	public static T GetConfig<T>(this IServiceProvider serviceProvider)
		where T : class
		=> serviceProvider.GetRequiredService<IOptions<T>>().Value;
}
