using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Services;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;
using FluentValidation;
using Moq;

namespace FinanceHub.Tests.Application.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repository = new();
    private readonly Mock<IValidator<CriarUsuarioRequest>> _validator = new();
    private readonly Mock<IValidator<AtualizarUsuarioRequest>> _atualizarValidator = new();
    private readonly Mock<IPasswordService> _passwordService = new();

    private UsuarioService CriarService()
    {
        return new UsuarioService(
            _repository.Object,
            _validator.Object,
            _atualizarValidator.Object,
            _passwordService.Object);
    }

    [Fact]
    public async Task CadastrarAsync_EmailJaCadastrado_DeveLancarException()
    {
        // Arrange

        var request = new CriarUsuarioRequest
        {
            Nome = "Igor",
            Email = "igor@email.com",
            Senha = "123456"
        };

        var usuarioExistente = new Usuario(
            request.Nome,
            request.Email,
            "hash");

        _repository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync(usuarioExistente);

        _passwordService
            .Setup(x => x.Hash(request.Senha))
            .Returns("hash");

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CadastrarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<EmailJaCadastradoException>();

        _repository.Verify(
            x => x.AdicionarAsync(It.IsAny<Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task CadastrarAsync_DadosValidos_DeveCriarUsuario()
    {
        // Arrange

        var request = new CriarUsuarioRequest
        {
            Nome = "Igor",
            Email = "igor@email.com",
            Senha = "123456"
        };

        _repository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        _passwordService
            .Setup(x => x.Hash(request.Senha))
            .Returns("hash-gerado");

        var service = CriarService();

        // Act

        var result = await service.CadastrarAsync(request);

        // Assert

        result.Nome.Should().Be(request.Nome);
        result.Email.Should().Be(request.Email);
        result.Id.Should().NotBeEmpty();

        _repository.Verify(
            x => x.AdicionarAsync(It.Is<Usuario>(u =>
                u.Nome == request.Nome &&
                u.Email == request.Email &&
                u.SenhaHash == "hash-gerado")),
            Times.Once);

        _passwordService.Verify(
            x => x.Hash(request.Senha),
            Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_UsuarioNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        _repository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.BuscarPorIdAsync(id);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioNaoEncontradoException>();
    }

    [Fact]
    public async Task BuscarPorIdAsync_UsuarioExistente_DeveRetornarResponse()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        _repository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        var service = CriarService();

        // Act

        var result = await service.BuscarPorIdAsync(usuario.Id);

        // Assert

        result.Id.Should().Be(usuario.Id);
        result.Nome.Should().Be(usuario.Nome);
        result.Email.Should().Be(usuario.Email);
    }

    [Fact]
    public async Task BuscarTodosAsync_DeveRetornarTodosOsUsuarios()
    {
        // Arrange

        var usuarios = new[]
        {
            new Usuario(
                "Igor",
                "igor@email.com",
                "hash"),

            new Usuario(
                "Joao",
                "joao@email.com",
                "hash")
        };

        _repository
            .Setup(x => x.BuscarTodosAsync())
            .ReturnsAsync(usuarios);

        var service = CriarService();

        // Act

        var result = (await service.BuscarTodosAsync()).ToList();

        // Assert

        result.Should().HaveCount(2);
        result.Select(x => x.Email)
            .Should()
            .Contain(new[] { "igor@email.com", "joao@email.com" });
    }

    [Fact]
    public async Task AtualizarAsync_UsuarioNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        var request = new AtualizarUsuarioRequest
        {
            Nome = "Igor",
            Email = "igor@email.com"
        };

        _repository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.AtualizarAsync(id, request);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioNaoEncontradoException>();
    }

    [Fact]
    public async Task AtualizarAsync_EmailDeOutroUsuario_DeveLancarException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var outroUsuario = new Usuario(
            "Joao",
            "joao@email.com",
            "hash");

        var request = new AtualizarUsuarioRequest
        {
            Nome = "Igor",
            Email = "joao@email.com"
        };

        _repository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        _repository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync(outroUsuario);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.AtualizarAsync(usuario.Id, request);

        // Assert

        await act.Should()
            .ThrowAsync<EmailJaCadastradoException>();

        _repository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DadosValidos_DeveAtualizar()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        var request = new AtualizarUsuarioRequest
        {
            Nome = "Igor Atualizado",
            Email = "igor.novo@email.com"
        };

        _repository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        _repository
            .Setup(x => x.BuscarPorEmailAsync(request.Email))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        await service.AtualizarAsync(usuario.Id, request);

        // Assert

        usuario.Nome.Should().Be(request.Nome);
        usuario.Email.Should().Be(request.Email);

        _repository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DesativarAsync_UsuarioNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        _repository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Usuario?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.DesativarAsync(id);

        // Assert

        await act.Should()
            .ThrowAsync<UsuarioNaoEncontradoException>();
    }

    [Fact]
    public async Task DesativarAsync_UsuarioExistente_DeveDesativar()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        _repository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync(usuario);

        var service = CriarService();

        // Act

        await service.DesativarAsync(usuario.Id);

        // Assert

        usuario.Ativo.Should().BeFalse();

        _repository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }
}