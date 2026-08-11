using FinanceHub.Api.Domain.Exceptions;
using FinanceHub.Api.ExceptionHandling.Responses;
using FinanceHub.Api.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace FinanceHub.Api.ExceptionHandling
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IExceptionHandler> _handlers;

        public ExceptionMiddleware(
            RequestDelegate next,
            IEnumerable<IExceptionHandler> handlers)
        {
            _next = next;
            _handlers = handlers;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var handler = _handlers
                    .FirstOrDefault(x => x.CanHandle(ex));

                if (handler is not null)
                {
                    await handler.HandleAsync(context, ex);
                    return;
                }

                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(
                    new ErrorResponse
                    {
                        Message = "Ocorreu um erro interno."
                    });
            }
        }
    }
}
