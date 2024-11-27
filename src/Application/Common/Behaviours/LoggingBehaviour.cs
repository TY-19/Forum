using Forum.Application.Users.Queries.Login;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Forum.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    private readonly Dictionary<Type, Action<TRequest>> _specificLoggers;

    public LoggingBehaviour(IConfiguration configuration,
        ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _specificLoggers = new()
        {
            { typeof(LoginRequest), LogLoginRequest }
        };
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {requestName}", typeof(TRequest).Name);

        if (_specificLoggers.ContainsKey(typeof(TRequest)))
            _specificLoggers[typeof(TRequest)].Invoke(request);
        else
            LogProperties(request);

        var response = await next();

        _logger.LogInformation("Handled {responseName}", typeof(TResponse).Name);
        return response;
    }

    private void LogProperties(TRequest request)
    {
        Type type = request.GetType();
        List<PropertyInfo> properties = new(type.GetProperties());
        foreach (PropertyInfo property in properties)
        {
            object? propertyValue = property.GetValue(request, null);
            _logger.LogInformation("{Property} : {@Value}", property.Name, propertyValue);
        }
    }

    private void LogLoginRequest(TRequest request)
    {
        if (request is LoginRequest loginRequest && !IsAllowedToLogSensitiveData())
        {
            _logger.LogInformation("{Property} : {@Value}", nameof(loginRequest.UserName), loginRequest.UserName);
            _logger.LogInformation("{Property} : {@Value}", nameof(loginRequest.Password), "#Sensitive data");
        }
        else
        {
            LogProperties(request);
        }
    }

    private bool IsAllowedToLogSensitiveData()
    {
        string? enableSensitiveDataLogging = _configuration?["EnableSensitiveDataLogging"];
        return enableSensitiveDataLogging == "true" || enableSensitiveDataLogging == "1";
    }
}
