using FinanceHub.Application.Interfaces.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace FinanceHub.Infrastructure.Cache;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out string? value);

        return Task.FromResult(value);
    }

    public Task SetAsync(
        string key,
        string value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default   )
    {
        _cache.Set(
            key,
            value,
            expiration);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);

        return Task.CompletedTask;
    }
}