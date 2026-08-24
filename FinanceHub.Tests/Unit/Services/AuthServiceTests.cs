using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Services;
using FinanceHub.Application.Validators;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace FinanceHub.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordService> _passwordService = new();
    private readonly LoginRequestValidator _validator = new();
    private readonly Mock<IRefreshTokenService> _refreshTokenService = new();

    private AuthService CriarService()
    {
        return new AuthService(
            _usuarioRepository.Object,
            _tokenService.Object,
            _passwordService.Object,
            _validator,
            _refreshTokenService.Object);
    }

    [Fact]
    public async Task LoginAsync_UsuarioNaoEncontrado_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "teste@email.com",
            Senha = "123456"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();
    }

    [Fact]
    public async Task LoginAsync_SenhaIncorreta_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var request = new LoginRequest
        {
            Email = usuario.Email,
            Senha = "senha-incorreta"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        _passwordService
            .Setup(x => x.Verify(
                request.Senha,
                usuario.SenhaHash))
            .Returns(false);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();
    }

    [Fact]
    public async Task LoginAsync_UsuarioInativo_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        usuario.Desativar();

        var request = new LoginRequest
        {
            Email = usuario.Email,
            Senha = "123456"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();

        _passwordService.Verify(
            x => x.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GerarToken(It.IsAny<Usuario>()),
            Times.Never);

        _refreshTokenService.Verify(
            x => x.CriarAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_CredenciaisValidas_DeveRetornarToken()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var request = new LoginRequest
        {
            Email = usuario.Email,
            Senha = "123456"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        _passwordService
            .Setup(x => x.Verify(
                request.Senha,
                usuario.SenhaHash))
            .Returns(true);

        _tokenService
            .Setup(x => x.GerarToken(usuario))
            .Returns("access-token-fake");

        _refreshTokenService
            .Setup(x => x.CriarAsync(usuario.Id))
            .ReturnsAsync("refresh-token-fake");

        var service = CriarService();

        // Act

        var result = await service.LoginAsync(request);

        // Assert

        result.AccessToken
            .Should()
            .Be("access-token-fake");

        result.RefreshToken
            .Should()
            .Be("refresh-token-fake");

        _passwordService.Verify(
            x => x.Verify(request.Senha, usuario.SenhaHash),
            Times.Once);

        _tokenService.Verify(
            x => x.GerarToken(usuario),
            Times.Once);

        _refreshTokenService.Verify(
            x => x.CriarAsync(usuario.Id),
            Times.Once);
    }

    [Fact]
    public async Task LoginAsync_EmailVazio_DeveLancarValidationException()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "",
            Senha = "123456"
        };

        var service = CriarService();

        // Act

        Func<Task> act =
            () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<FluentValidation.ValidationException>();

        _usuarioRepository.Verify(
            x => x.BuscarPorEmailAsync(It.IsAny<string>()),
            Times.Never);

        _passwordService.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GerarToken(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_EmailInvalido_DeveLancarValidationException()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "email-invalido",
            Senha = "123456"
        };

        var service = CriarService();

        // Act

        Func<Task> act =
            () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<FluentValidation.ValidationException>();

        _usuarioRepository.Verify(
            x => x.BuscarPorEmailAsync(It.IsAny<string>()),
            Times.Never);

        _passwordService.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GerarToken(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_SenhaVazia_DeveLancarValidationException()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "teste@email.com",
            Senha = ""
        };

        var service = CriarService();

        // Act

        Func<Task> act =
            () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<FluentValidation.ValidationException>();

        _usuarioRepository.Verify(
            x => x.BuscarPorEmailAsync(It.IsAny<string>()),
            Times.Never);

        _passwordService.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GerarToken(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_UsuarioNaoEncontrado_NaoDeveGerarToken()
    {
        // Arrange

        var request = new LoginRequest
        {
            Email = "teste@email.com",
            Senha = "123456"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act =
            () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();

        _passwordService.Verify(
            x => x.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _tokenService.Verify(
            x => x.GerarToken(It.IsAny<Usuario>()),
            Times.Never);

        _refreshTokenService.Verify(
            x => x.CriarAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_SenhaIncorreta_NaoDeveGerarToken()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var request = new LoginRequest
        {
            Email = usuario.Email,
            Senha = "senha-incorreta"
        };

        _usuarioRepository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync(usuario);

        _passwordService
            .Setup(x => x.Verify(
                request.Senha,
                usuario.SenhaHash))
            .Returns(false);

        var service = CriarService();

        // Act

        Func<Task> act =
            () => service.LoginAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();

        _tokenService.Verify(
            x => x.GerarToken(It.IsAny<Usuario>()),
            Times.Never);

        _refreshTokenService.Verify(
            x => x.CriarAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_TokenValido_DeveRetornarNovosTokens()
    {
        // Arrange

        var usuario =
            new Usuario(
                "Igor",
                "refresh@teste.com",
                "hash");

        var refreshToken =
            new RefreshToken(
                usuario.Id,
                "hash-refresh",
                DateTime.UtcNow.AddDays(30));

        typeof(RefreshToken)
            .GetProperty(nameof(RefreshToken.Usuario))!
            .SetValue(
                refreshToken,
                usuario);

        var request =
            new RefreshTokenRequest
            {
                RefreshToken = "refresh-original"
            };

        _refreshTokenService
            .Setup(x =>
                x.BuscarValidoAsync(
                    request.RefreshToken))
            .ReturnsAsync(refreshToken);

        _tokenService
            .Setup(x =>
                x.GerarToken(
                    It.IsAny<Usuario>()))
            .Returns("novo-access-token");

        _refreshTokenService
            .Setup(x =>
                x.CriarAsync(usuario.Id))
            .ReturnsAsync("novo-refresh-token");

        var service =
            CriarService();

        // Act

        var result =
            await service.RefreshAsync(request);

        // Assert

        result.AccessToken
            .Should()
            .Be("novo-access-token");

        result.RefreshToken
            .Should()
            .Be("novo-refresh-token");

        _tokenService.Verify(
            x => x.GerarToken(
                It.Is<Usuario>(u =>
                    u != null &&
                    u.Id == usuario.Id)),
            Times.Once);

        _refreshTokenService.Verify(
            x => x.BuscarValidoAsync(
                request.RefreshToken),
            Times.Once);

        _tokenService.Verify(
            x => x.GerarToken(usuario),
            Times.Once);

        _refreshTokenService.Verify(
            x => x.RevogarAsync(refreshToken),
            Times.Once);

        _refreshTokenService.Verify(
            x => x.CriarAsync(usuario.Id),
            Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_TokenInvalido_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange

        var request =
            new RefreshTokenRequest
            {
                RefreshToken = "token-invalido"
            };

        _refreshTokenService
            .Setup(x =>
                x.BuscarValidoAsync(
                    request.RefreshToken))
            .ReturnsAsync((RefreshToken?)null);

        var service =
            CriarService();

        // Act

        Func<Task> act =
            () => service.RefreshAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();

        _tokenService.Verify(
            x => x.GerarToken(
                It.IsAny<Usuario>()),
            Times.Never);

        _refreshTokenService.Verify(
            x => x.RevogarAsync(
                It.IsAny<RefreshToken>()),
            Times.Never);

        _refreshTokenService.Verify(
            x => x.CriarAsync(
                It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_TokenVazio_DeveLancarCredenciaisInvalidasException()
    {
        // Arrange

        var request =
            new RefreshTokenRequest
            {
                RefreshToken = ""
            };

        var service =
            CriarService();

        // Act

        Func<Task> act =
            () => service.RefreshAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<CredenciaisInvalidasException>();

        _refreshTokenService.Verify(
            x => x.BuscarValidoAsync(
                It.IsAny<string>()),
            Times.Never);
    }
}