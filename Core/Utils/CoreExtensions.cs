using Core.Database;
using Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Utils;

public static class CoreExtensions
{
	public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationContext>(builder =>
		{
			builder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
		});
		services.AddScoped<IUserGroupRepository, UserGroupRepository>();
		services.AddScoped<IUserStateRepository, UserStateRepository>();
		services.AddScoped<IPasswordHasher, PasswordHasher>();
		services.AddScoped<IUsersRepository, UsersRepository>();
		services.AddScoped<IUsersService, UsersService>();
		services.AddScoped<IDbStore>(provider => provider.GetRequiredService<ApplicationContext>());
		return services;
	}
}