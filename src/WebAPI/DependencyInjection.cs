using Forum.Application.Common.Interfaces.Repositories;
using Forum.Infrastructure.Repositories;
using Forum.WebAPI.Infrastructure;

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

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddScoped<IForumRepository, ForumRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();

        return services;
    }
}
