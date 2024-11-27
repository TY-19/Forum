using Forum.Application.Common.Interfaces;
using Forum.WebAPI.Common;
using Forum.WebAPI.Common.Authorization;
using Forum.WebAPI.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace Forum.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, new JwtBearerConfigureOptions(configuration).Configure);
        services.ConfigureOptions<AuthenticationConfigureOptions>();

        services.AddAuthorization();
        services.ConfigureOptions<AuthorizationConfigureOptions>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddSerilog(new LoggerConfigureOptions(configuration).Configure);

        services.AddCors();
        services.ConfigureOptions<CorsConfigureOptions>();

        services.AddEndpointsApiExplorer();
        services.AddControllers();
        services.ConfigureOptions<JsonConfigureOptions>();

        services.AddSwaggerGen();
        services.ConfigureOptions<SwaggerConfigureOptions>();


        services.AddTransient<IAuthorizationHandler, PermissionHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
