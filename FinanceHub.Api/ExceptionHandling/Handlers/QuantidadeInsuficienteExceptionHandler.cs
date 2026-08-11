using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.ExceptionHandling.Responses;

namespace FinanceHub.Api.ExceptionHandling.Handlers;

public class QuantidadeInsuficienteExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is QuantidadeInsuficienteException;
    }

    public async Task HandleAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        await context.Response.WriteAsJsonAsync(
            new ErrorResponse
            {
                Message = exception.Message
            });
    }
}