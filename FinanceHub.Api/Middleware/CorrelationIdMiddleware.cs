namespace FinanceHub.Api.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName =
        "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        var correlationId =
            context.Request.Headers[HeaderName]
                .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId =
                Guid.NewGuid().ToString();
        }

        context.Items[HeaderName] =
            correlationId;

        context.Response.Headers[HeaderName] =
            correlationId;

        using var scope =
             _logger.BeginScope(
                 new Dictionary<string, object>
                 {
                     ["CorrelationId"] =
                         correlationId
                 });

        _logger.LogInformation(
            "Processando requisição. Method: {Method}, Path: {Path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);
    }
}