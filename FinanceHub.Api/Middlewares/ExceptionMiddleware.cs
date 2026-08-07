using FinanceHub.Api.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace FinanceHub.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is ValidationException validationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Response.ContentType = "application/json";

                    var errors = validationException.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray());

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            errors
                        }));

                    return;
                }

                if (ex is UsuarioNaoEncontradoException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            message = ex.Message
                        }));

                    return;
                }

                if (ex is EmailJaCadastradoException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            new
                            {
                                message = ex.Message
                            }
                        )
                    );

                    return;
                }

                if (ex is UsuarioJaDesativadoException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = ex.Message
                    });

                    return;
                }

                if (ex is TickerJaCadastradoException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(
                            new
                            {
                                message = ex.Message
                            }
                        )
                    );

                    return;
                }

                if (ex is BolsaNaoEncontradaException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            message = ex.Message
                        }));

                    return;
                }

                if (ex is AtivoNaoEncontradoException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(new
                        {
                            message = ex.Message
                        }));

                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(new
                    {
                        message = "Ocorreu um erro interno."
                    })
                );
            }
        }
    }
}
