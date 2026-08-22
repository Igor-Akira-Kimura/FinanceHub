//using FinanceHub.Domain.Exceptions;
//using FinanceHub.Api.ExceptionHandling.Responses;
//using FluentValidation;
//using System.Text.Json;

//namespace FinanceHub.Api.ExceptionHandling
//{
//    public class ExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IEnumerable<IExceptionHandler> _handlers;

//        public ExceptionMiddleware(
//            RequestDelegate next,
//            IEnumerable<IExceptionHandler> handlers)
//        {
//            _next = next;
//            _handlers = handlers;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                var handler = _handlers
//                    .FirstOrDefault(x => x.CanHandle(ex));

//                if (handler is not null)
//                {
//                    await handler.HandleAsync(context, ex);
//                    return;
//                }

//                context.Response.StatusCode =
//                    StatusCodes.Status500InternalServerError;

//                await context.Response.WriteAsJsonAsync(
//                    new ErrorResponse
//                    {
//                        Message = "Ocorreu um erro interno."
//                    });
//            }
//        }
//    }
//}


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
                _logger.LogError(
                    ex,
                    "Erro não tratado ao processar a requisição.");

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