using FinanceHub.Api.DependencyInjection;
using FinanceHub.Api.ExceptionHandling;
using FinanceHub.Api.Middleware;
using FinanceHub.Application.Interfaces.Cache;
using FinanceHub.Infrastructure.Cache;
using StackExchange.Redis;

namespace FinanceHub.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder =
            WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddHttpContextAccessor();

        // =========================================
        // APPLICATION / INFRASTRUCTURE
        // =========================================

        builder.Services
            .AddSwaggerDocumentation()
            .AddValidation()
            .AddApplicationServices()
            .AddRepositories()
            .AddDatabase(builder.Configuration)
            .AddRedis(builder.Configuration)
            .AddJwtAuthentication(builder.Configuration)
            .AddObservability()
            .AddExceptionHandlers()
            .AddHealthChecks();

        var app = builder.Build();

        app.UseMiddleware<CorrelationIdMiddleware>();

        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health");

        app.MapPrometheusScrapingEndpoint();

        app.Run();
    }
}