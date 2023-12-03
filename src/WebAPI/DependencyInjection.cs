using Forum.Application.Common.Interfaces.Repositories;
using Forum.Infrastructure.Repositories;

namespace Forum.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddCors(opt => opt.AddPolicy(name: "FreeCorsPolicy", cfg =>
        {
            cfg.AllowAnyHeader();
            cfg.AllowAnyMethod();
            cfg.WithOrigins("*");
        }));

        services.AddScoped<IForumRepository, ForumRepository>();

        return services;
    }
}
