using Forum.Application.Common.Interfaces;
using Forum.Infrastructure.Data;
using Forum.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Forum.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ForumDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IForumDbContext>(provider => provider.GetRequiredService<ForumDbContext>());

        services.AddScoped<ForumDbContextInitialiser>();

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ForumDbContext>()
            .AddApiEndpoints();
        services.ConfigureOptions<IdentityConfigureOptions>();

        services.AddScoped<IUserManager, ForumUserManager>();
        services.AddScoped<IRoleManager, ForumRoleManager>();
        services.AddScoped<IJwtHandler, JwtHandler>();

        return services;
    }
}
