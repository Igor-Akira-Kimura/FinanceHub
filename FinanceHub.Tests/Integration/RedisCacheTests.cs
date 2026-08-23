using FinanceHub.Infrastructure.Cache;
using FluentAssertions;
using StackExchange.Redis;

namespace FinanceHub.Tests.Integration;

public class RedisCacheTests
{
    [Fact]
    public async Task
        SetEGet_DeveRetornarValorDoRedis()
    {
        await using var connection =
            await ConnectionMultiplexer.ConnectAsync(
                "localhost:6379");

        var cache =
            new RedisCacheService(connection);

        var key =
            $"teste:{Guid.NewGuid()}";

        var value =
            """{"preco":100}""";

        await cache.SetAsync(
            key,
            value,
            TimeSpan.FromMinutes(5));

        var resultado =
            await cache.GetAsync(key);

        resultado.Should().Be(value);

        await cache.RemoveAsync(key);
    }

    [Fact]
    public async Task
        Get_ChaveInexistente_DeveRetornarNull()
    {
        await using var connection =
            await ConnectionMultiplexer.ConnectAsync(
                "localhost:6379");

        var cache =
            new RedisCacheService(connection);

        var key =
            $"teste:{Guid.NewGuid()}";

        var resultado =
            await cache.GetAsync(key);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task
        Remove_DeveExcluirValorDoRedis()
    {
        await using var connection =
            await ConnectionMultiplexer.ConnectAsync(
                "localhost:6379");

        var cache =
            new RedisCacheService(connection);

        var key =
            $"teste:{Guid.NewGuid()}";

        var value =
            """{"preco":100}""";

        await cache.SetAsync(
            key,
            value,
            TimeSpan.FromMinutes(5));

        await cache.RemoveAsync(key);

        var resultado =
            await cache.GetAsync(key);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task
        SetComExpiracao_DeveRetornarNullAposExpirar()
    {
        await using var connection =
            await ConnectionMultiplexer.ConnectAsync(
                "localhost:6379");

        var cache =
            new RedisCacheService(connection);

        var key =
            $"teste:{Guid.NewGuid()}";

        var value =
            """{"preco":100}""";

        await cache.SetAsync(
            key,
            value,
            TimeSpan.FromSeconds(1));

        var antesDeExpirar =
            await cache.GetAsync(key);

        await Task.Delay(
            TimeSpan.FromSeconds(2));

        var depoisDeExpirar =
            await cache.GetAsync(key);

        antesDeExpirar.Should().Be(value);

        depoisDeExpirar.Should().BeNull();
    }

    [Fact]
    public async Task
        Set_MesmaChave_DeveSobrescreverValorAnterior()
    {
        await using var connection =
            await ConnectionMultiplexer.ConnectAsync(
                "localhost:6379");

        var cache =
            new RedisCacheService(connection);

        var key =
            $"teste:{Guid.NewGuid()}";

        await cache.SetAsync(
            key,
            """{"preco":100}""",
            TimeSpan.FromMinutes(5));

        await cache.SetAsync(
            key,
            """{"preco":150}""",
            TimeSpan.FromMinutes(5));

        var resultado =
            await cache.GetAsync(key);

        resultado.Should()
            .Be("""{"preco":150}""");

        await cache.RemoveAsync(key);
    }
}