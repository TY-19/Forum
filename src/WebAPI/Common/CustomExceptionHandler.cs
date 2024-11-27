using Forum.Application.Common.Exceptions;
using Forum.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace Forum.WebAPI.Common;

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
        }
        else
        {
            await HandleDefaultExceptionAsync(httpContext, exception);
        }
        return true;
    }

    private async Task HandleValidationExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var ex = (CustomValidationException)exception;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new CustomResponse()
        {
            Message = "One or more validation errors have occured",
            Succeeded = false,
            Errors = ex.Errors
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
