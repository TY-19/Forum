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

        services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ForumDbContext>()
            .AddApiEndpoints();

        //services.AddTransient<IIdentityService, IdentityService>();

        //services.AddAuthorization(options =>
        //    options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        return services;
    }
}
