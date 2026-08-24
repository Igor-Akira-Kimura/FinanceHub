using FinanceHub.Domain.Entities;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Domain;

public class RefreshTokenTests
{
    [Fact]
    public void CriarRefreshToken_DeveCriarTokenAtivo()
    {
        var usuarioId =
            Guid.NewGuid();

        var expiresAt =
            DateTime.UtcNow.AddDays(30);

        var refreshToken =
            new RefreshToken(
                usuarioId,
                "hash-token",
                expiresAt);

        refreshToken.Id
            .Should()
            .NotBe(Guid.Empty);

        refreshToken.UsuarioId
            .Should()
            .Be(usuarioId);

        refreshToken.TokenHash
            .Should()
            .Be("hash-token");

        refreshToken.IsRevoked
            .Should()
            .BeFalse();

        refreshToken.IsExpired
            .Should()
            .BeFalse();

        refreshToken.IsActive
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Revogar_DeveDeixarTokenInativo()
    {
        var refreshToken =
            CriarRefreshToken();

        refreshToken.Revogar();

        refreshToken.IsRevoked
            .Should()
            .BeTrue();

        refreshToken.RevokedAt
            .Should()
            .NotBeNull();

        refreshToken.IsActive
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Revogar_DuasVezes_NaoDeveAlterarEstado()
    {
        var refreshToken =
            CriarRefreshToken();

        refreshToken.Revogar();

        var revokedAt =
            refreshToken.RevokedAt;

        refreshToken.Revogar();

        refreshToken.RevokedAt
            .Should()
            .Be(revokedAt);
    }

    [Fact]
    public void TokenExpirado_DeveEstarInativo()
    {
        var refreshToken =
            CriarRefreshToken(
                DateTime.UtcNow.AddSeconds(-1));

        refreshToken.IsExpired
            .Should()
            .BeTrue();

        refreshToken.IsActive
            .Should()
            .BeFalse();
    }

    [Fact]
    public void UsuarioIdVazio_DeveLancarException()
    {
        var act = () =>
            new RefreshToken(
                Guid.Empty,
                "hash-token",
                DateTime.UtcNow.AddDays(30));

        act.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void TokenHashVazio_DeveLancarException()
    {
        var act = () =>
            new RefreshToken(
                Guid.NewGuid(),
                "",
                DateTime.UtcNow.AddDays(30));

        act.Should()
            .Throw<ArgumentException>();
    }

    private static RefreshToken CriarRefreshToken(
        DateTime? expiresAt = null)
    {
        return new RefreshToken(
            Guid.NewGuid(),
            "hash-token",
            expiresAt ??
            DateTime.UtcNow.AddDays(30));
    }
}