using Dawn;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
	{
		Guard.Argument(configuration).NotNull();
		var config = Helpers.MySql.Config.Defaults;
		configuration.Bind(config);
		return AddDbConnection(services, config);
	}

	public static IServiceCollection AddDbConnection(this IServiceCollection services, string connectionString)
	{
		Guard.Argument(connectionString).NotNull().NotEmpty().NotWhiteSpace();
		var config = Helpers.MySql.Config.Parse(connectionString, provider: null);
		return AddDbConnection(services, config);
	}

	public static IServiceCollection AddDbConnection(this IServiceCollection services, string server, uint port, string database, string userId, string password, bool secure)
	{
		var config = new Helpers.MySql.Config(Server: server, Port: port, Database: database, UserId: userId, Password: password, Secure: secure);
		return AddDbConnection(services, config);
	}

	public static IServiceCollection AddDbConnection(this IServiceCollection services, Helpers.MySql.Config config)
	{
		Guard.Argument(config).NotNull();
		return services.AddSingleton(config.DbConnection);
	}
}
