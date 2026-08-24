using FinanceHub.Api.DependencyInjection;
using FinanceHub.Api.ExceptionHandling;
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

        //// =========================================
        //// REDIS
        //// =========================================

        //var redisConnectionString =
        //    builder.Configuration[
        //        "Redis:ConnectionString"]
        //    ?? throw new InvalidOperationException(
        //        "Redis:ConnectionString não configurada.");

        //builder.Services.AddSingleton<IConnectionMultiplexer>(
        //    ConnectionMultiplexer.Connect(
        //        redisConnectionString));

        //builder.Services.AddSingleton<ICacheService,
        //    RedisCacheService>();

        // =========================================
        // CACHE
        // =========================================

        var redisConnectionString =
            builder.Configuration[
                "Redis:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(
                    redisConnectionString));

            builder.Services.AddSingleton<ICacheService,
                RedisCacheService>();
        }
        else
        {
            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton<ICacheService,
                MemoryCacheService>();
        }

        // =========================================
        // APPLICATION / INFRASTRUCTURE
        // =========================================

        builder.Services
            .AddSwaggerDocumentation()
            .AddValidation()
            .AddApplicationServices()
            .AddRepositories()
            .AddDatabase(builder.Configuration)
            .AddJwtAuthentication(builder.Configuration)
            .AddExceptionHandlers()
            .AddHealthChecks();

        var app = builder.Build();

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

        app.Run();
    }
}