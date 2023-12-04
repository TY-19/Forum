using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Forum.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling {requestName}", typeof(TRequest).Name);
        Type type = request.GetType();
        List<PropertyInfo> properties = new(type.GetProperties());
        foreach (PropertyInfo property in properties)
        {
            object? propertyValue = property.GetValue(request, null);
            logger.LogInformation("{Property} : {@Value}", property.Name, propertyValue);
        }
        var response = await next();

        logger.LogInformation("Handled {responseName}", typeof(TResponse).Name);
        return response;
    }
}
