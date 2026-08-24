using FinanceHub.Api.ExceptionHandling.Responses;

namespace FinanceHub.Api.ExceptionHandling
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IExceptionHandler> _handlers;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            RequestDelegate next,
            IEnumerable<IExceptionHandler> handlers,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _handlers = handlers;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId =
                    context.Items["X-Correlation-ID"]?.ToString();

                _logger.LogError(
                    ex,
                    "Erro não tratado. CorrelationId: {CorrelationId}, Method: {Method}, Path: {Path}",
                    correlationId,
                    context.Request.Method,
                    context.Request.Path);

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