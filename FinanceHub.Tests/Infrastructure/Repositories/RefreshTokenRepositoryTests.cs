using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class RefreshTokenRepositoryTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public RefreshTokenRepositoryTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task AdicionarAsync_DevePersistirRefreshToken()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new RefreshTokenRepository(context);

        var usuario =
            new Usuario(
                "Igor",
                "refresh@test.com",
                "hash");

        context.Usuarios.Add(usuario);

        await context.SaveChangesAsync();

        var refreshToken =
            CriarRefreshToken(usuario.Id);

        await repository.AdicionarAsync(
            refreshToken);

        var salvo =
            await context.RefreshTokens
                .SingleAsync();

        salvo.Id
            .Should()
            .Be(refreshToken.Id);

        salvo.UsuarioId
            .Should()
            .Be(usuario.Id);

        salvo.TokenHash
            .Should()
            .Be(refreshToken.TokenHash);
    }

    [Fact]
    public async Task BuscarPorTokenHashAsync_DeveRetornarToken()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new RefreshTokenRepository(context);

        var usuario =
            new Usuario(
                "Igor",
                "buscar-refresh@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(usuario.Id);

        context.Usuarios.Add(usuario);
        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        var resultado =
            await repository
                .BuscarPorTokenHashAsync(
                    refreshToken.TokenHash);

        resultado.Should()
            .NotBeNull();

        resultado!.Id
            .Should()
            .Be(refreshToken.Id);

        resultado.Usuario
            .Should()
            .NotBeNull();

        resultado.Usuario.Id
            .Should()
            .Be(usuario.Id);
    }

    [Fact]
    public async Task BuscarPorTokenHashAsync_TokenInexistente_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new RefreshTokenRepository(context);

        var resultado =
            await repository
                .BuscarPorTokenHashAsync(
                    "hash-inexistente");

        resultado.Should()
            .BeNull();
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_DevePersistirRevogacao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new RefreshTokenRepository(context);

        var usuario =
            new Usuario(
                "Igor",
                "revogar-refresh@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(usuario.Id);

        context.Usuarios.Add(usuario);
        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();

        refreshToken.Revogar();

        await repository
            .SalvarAlteracoesAsync();

        var salvo =
            await context.RefreshTokens
                .AsNoTracking()
                .SingleAsync();

        salvo.RevokedAt
            .Should()
            .NotBeNull();
    }

    private static RefreshToken CriarRefreshToken(
        Guid usuarioId)
    {
        return new RefreshToken(
            usuarioId,
            Guid.NewGuid().ToString("N"),
            DateTime.UtcNow.AddDays(30));
    }
}