using Forum.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebAPI.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    public CustomExceptionHandler()
    {
        _exceptionHandlers = new()
        {
            { typeof(CustomValidationException), HandleValidationExceptionAsync }
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();
        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, Task>? value))
        {
            await value!.Invoke(httpContext, exception);
            return true;
        }
        else
        {
            await HandleDefaultExceptionAsync(httpContext, exception);
            return true;
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var ex = (CustomValidationException)exception;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(ex.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }

    private static async Task HandleDefaultExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new 
        {
            Status = StatusCodes.Status400BadRequest,
            exception.Message,
            exception.StackTrace,
            InnerException = exception.InnerException?.Message

        });
    }
}
