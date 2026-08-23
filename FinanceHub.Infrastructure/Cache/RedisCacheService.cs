using FinanceHub.Application.Interfaces.Cache;
using StackExchange.Redis;

namespace FinanceHub.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(
        IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<string?> GetAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var database =
            _redis.GetDatabase();

        var value =
            await database.StringGetAsync(key);

        return value.IsNull
            ? null
            : value.ToString();
    }

    public async Task SetAsync(
        string key,
        string value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var database =
            _redis.GetDatabase();

        await database.StringSetAsync(
            key,
            value,
            expiration);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var database =
            _redis.GetDatabase();

        await database.KeyDeleteAsync(key);
    }
}