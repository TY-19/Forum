using FluentValidation;
using Forum.Application.Common.Behaviours;
using Forum.Application.Common.Helpers;
using Forum.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Forum.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddScoped(typeof(ISearchHelper<>), typeof(SearchHelper<>));

        return services;
    }
}
