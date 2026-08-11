using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.ExceptionHandling.Responses;

namespace FinanceHub.Api.ExceptionHandling.Handlers
{
    public class UsuarioJaDesativadoExceptionHandler : IExceptionHandler
    {
        public bool CanHandle(Exception exception)
        {
            return exception is UsuarioJaDesativadoException;
        }

        public async Task HandleAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;

            await context.Response.WriteAsJsonAsync(
                new ErrorResponse
                {
                    Message = exception.Message
                });
        }
    }
}
