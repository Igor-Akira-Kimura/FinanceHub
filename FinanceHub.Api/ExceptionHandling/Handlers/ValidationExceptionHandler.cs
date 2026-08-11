using FinanceHub.Api.ExceptionHandling;
using FluentValidation;

namespace FinanceHub.Api.ExceptionHandling.Handlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception)
    {
        return exception is ValidationException;
    }

    public async Task HandleAsync(
        HttpContext context,
        Exception exception)
    {
        var validationException = (ValidationException)exception;

        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());

        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        await context.Response.WriteAsJsonAsync(new
        {
            errors
        });
    }
}