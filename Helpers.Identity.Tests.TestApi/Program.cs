namespace Helpers.Identity.Tests.TestApi;

public class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();

		builder.Services
			.AddAuthenticationAuthorization(
				authority: new Uri("https://identityserver"),
				scope: "api1");

		var app = builder.Build();

		app
			.UseAuthentication()
			.UseAuthorization();

		app
			.MapControllers()
			.RequireAuthorization("ApiScope");

		app.Run();
	}
}
