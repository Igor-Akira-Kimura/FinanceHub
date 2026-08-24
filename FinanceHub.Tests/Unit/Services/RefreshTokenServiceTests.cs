using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Security;
using FluentAssertions;
using Moq;

namespace FinanceHub.Tests.Unit.Security;

public class RefreshTokenServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _repository = new();

    private RefreshTokenService CriarService()
    {
        return new RefreshTokenService(
            _repository.Object);
    }

    [Fact]
    public async Task BuscarValidoAsync_TokenValido_DeveRetornarToken()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "refresh@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(usuario);

        _repository
            .Setup(x =>
                x.BuscarPorTokenHashAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var service = CriarService();

        // Act

        var resultado =
            await service.BuscarValidoAsync(
                "token-original");

        // Assert

        resultado.Should()
            .NotBeNull();

        resultado.Should()
            .Be(refreshToken);
    }

    [Fact]
    public async Task BuscarValidoAsync_TokenInexistente_DeveRetornarNull()
    {
        // Arrange

        _repository
            .Setup(x =>
                x.BuscarPorTokenHashAsync(
                    It.IsAny<string>()))
            .ReturnsAsync((RefreshToken?)null);

        var service = CriarService();

        // Act

        var resultado =
            await service.BuscarValidoAsync(
                "token-inexistente");

        // Assert

        resultado.Should()
            .BeNull();
    }

    [Fact]
    public async Task BuscarValidoAsync_TokenRevogado_DeveRetornarNull()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "revogado@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(usuario);

        refreshToken.Revogar();

        _repository
            .Setup(x =>
                x.BuscarPorTokenHashAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var service = CriarService();

        // Act

        var resultado =
            await service.BuscarValidoAsync(
                "token-revogado");

        // Assert

        resultado.Should()
            .BeNull();
    }

    [Fact]
    public async Task BuscarValidoAsync_TokenExpirado_DeveRetornarNull()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "expirado@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(
                usuario,
                DateTime.UtcNow.AddMinutes(-1));

        _repository
            .Setup(x =>
                x.BuscarPorTokenHashAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var service = CriarService();

        // Act

        var resultado =
            await service.BuscarValidoAsync(
                "token-expirado");

        // Assert

        resultado.Should()
            .BeNull();
    }

    [Fact]
    public async Task BuscarValidoAsync_UsuarioInativo_DeveRetornarNull()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "inativo@test.com",
                "hash");

        usuario.Desativar();

        var refreshToken =
            CriarRefreshToken(usuario);

        _repository
            .Setup(x =>
                x.BuscarPorTokenHashAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var service = CriarService();

        // Act

        var resultado =
            await service.BuscarValidoAsync(
                "token-usuario-inativo");

        // Assert

        resultado.Should()
            .BeNull();
    }

    [Fact]
    public async Task BuscarValidoAsync_DeveBuscarPeloHashDoToken()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "hash@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(usuario);

        _repository
            .Setup(x =>
                x.BuscarPorTokenHashAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(refreshToken);

        var service = CriarService();

        var tokenOriginal =
            "token-original";

        // Act

        await service.BuscarValidoAsync(
            tokenOriginal);

        // Assert

        _repository.Verify(
            x => x.BuscarPorTokenHashAsync(
                It.Is<string>(hash =>
                    hash != tokenOriginal)),
            Times.Once);
    }

    [Fact]
    public async Task RevogarAsync_DeveRevogarTokenESalvarAlteracoes()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "revogar@test.com",
                "hash");

        var refreshToken =
            CriarRefreshToken(usuario);

        var service = CriarService();

        // Act

        await service.RevogarAsync(
            refreshToken);

        // Assert

        refreshToken.RevokedAt
            .Should()
            .NotBeNull();

        _repository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    private static RefreshToken CriarRefreshToken(
        Usuario usuario,
        DateTime? expiresAt = null)
    {
        var refreshToken =
            new RefreshToken(
                usuario.Id,
                Guid.NewGuid().ToString("N"),
                expiresAt ??
                DateTime.UtcNow.AddDays(30));

        // O setter de Usuario é private.
        // O EF Core preencheria essa navegação ao carregar
        // o RefreshToken com Include(x => x.Usuario).

        typeof(RefreshToken)
            .GetProperty(nameof(RefreshToken.Usuario))!
            .SetValue(
                refreshToken,
                usuario);

        return refreshToken;
    }
}