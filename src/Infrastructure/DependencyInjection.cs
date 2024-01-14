using Forum.Application.Common.Interfaces;
using Forum.Infrastructure.Common.Interfaces;
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
        ConfigureDbContext(services, configuration);

        services.AddScoped<IForumDbContext>(provider => provider.GetRequiredService<ForumDbContext>());

        services.AddScoped<ForumDbContextInitialiser>();

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ForumDbContext>()
            .AddApiEndpoints();
        services.ConfigureOptions<IdentityConfigureOptions>();

        services.AddScoped<IUserManager, ForumUserManager>();
        services.AddScoped<IRoleManager, ForumRoleManager>();
        services.AddScoped<IJwtHandler, JwtHandler>();

        services.AddScoped<IDatabaseHelper, DatabaseHelper>();

        return services;
    }

    private static void ConfigureDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        string? enableSensitiveDataLogging = configuration?["EnableSensitiveDataLogging"];
        services.AddDbContext<ForumDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            if (enableSensitiveDataLogging == "true" || enableSensitiveDataLogging == "1")
                options.EnableSensitiveDataLogging();
        });
    }
}
