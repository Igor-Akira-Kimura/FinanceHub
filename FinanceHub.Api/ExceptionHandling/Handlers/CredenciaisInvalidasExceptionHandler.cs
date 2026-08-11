using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.ExceptionHandling.Responses;
using FinanceHub.Api.Exceptions;

namespace FinanceHub.Api.ExceptionHandling.Handlers;

public class CredenciaisInvalidasExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is CredenciaisInvalidasException;
    }

    public async Task HandleAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await context.Response.WriteAsJsonAsync(
            new ErrorResponse
            {
                Message = exception.Message
            });
    }
}