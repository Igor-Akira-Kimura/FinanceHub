using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests.Auth;
using FinanceHub.Application.Services;
using FinanceHub.Application.Validators;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace FinanceHub.Tests.Application.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<IPasswordService> _passwordService = new();
    private readonly LoginRequestValidator _validator = new();

    private AuthService CriarService()
    {
        return new AuthService(
            _usuarioRepository.Object,
            _tokenService.Object,
            _passwordService.Object,
            _validator);
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
            .Returns("token-fake");

        var service = CriarService();

        // Act

        var result = await service.LoginAsync(request);

        // Assert

        result.Token.Should().Be("token-fake");

        _passwordService.Verify(
            x => x.Verify(request.Senha, usuario.SenhaHash),
            Times.Once);

        _tokenService.Verify(
            x => x.GerarToken(usuario),
            Times.Once);
    }
}