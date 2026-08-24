using FinanceHub.Application.Interfaces.Cache;
using FinanceHub.Infrastructure.Cache;
using StackExchange.Redis;

namespace FinanceHub.Api.DependencyInjection;

public static class RedisExtensions
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisConnectionString =
            configuration["Redis:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(
                    redisConnectionString));

            services.AddSingleton<ICacheService,
                RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();

            services.AddSingleton<ICacheService,
                MemoryCacheService>();
        }

        return services;
    }
}