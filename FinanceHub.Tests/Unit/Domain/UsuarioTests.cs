using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Domain;

public class UsuarioTests
{
    [Fact]
    public void Construtor_DeveCriarUsuarioComDadosInformados()
    {
        // Arrange

        var nome = "Igor";
        var email = "igor@email.com";
        var senhaHash = "hash-da-senha";

        // Act

        var usuario = new Usuario(
            nome,
            email,
            senhaHash);

        // Assert

        usuario.Id.Should().NotBeEmpty();

        usuario.Nome.Should().Be(nome);

        usuario.Email.Should().Be(email);

        usuario.SenhaHash.Should().Be(senhaHash);

        usuario.Ativo.Should().BeTrue();

        usuario.DataCriacao
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));

        usuario.DataAtualizacao.Should().BeNull();

        usuario.Carteiras.Should().BeEmpty();
    }

    [Fact]
    public void Construtor_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Usuario(
            "",
            "igor@email.com",
            "hash");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_NomeSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Usuario(
            "   ",
            "igor@email.com",
            "hash");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_EmailVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Usuario(
            "Igor",
            "",
            "hash");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Fact]
    public void Construtor_EmailSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Usuario(
            "Igor",
            "   ",
            "hash");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Fact]
    public void Construtor_SenhaHashVazia_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Usuario(
            "Igor",
            "igor@email.com",
            "");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("senhaHash");
    }

    [Fact]
    public void Construtor_SenhaHashSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Usuario(
            "Igor",
            "igor@email.com",
            "   ");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("senhaHash");
    }

    [Fact]
    public void Atualizar_DeveAlterarNomeEEmail()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        // Act

        usuario.Atualizar(
            "Novo Nome",
            "novo@email.com");

        // Assert

        usuario.Nome.Should().Be("Novo Nome");

        usuario.Email.Should().Be("novo@email.com");
    }

    [Fact]
    public void Atualizar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        // Act

        usuario.Atualizar(
            "Novo Nome",
            "novo@email.com");

        // Assert

        usuario.DataAtualizacao
            .Should()
            .NotBeNull();

        usuario.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Atualizar_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        // Act

        Action act = () => usuario.Atualizar(
            "",
            "novo@email.com");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Atualizar_EmailVazio_DeveLancarArgumentException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        // Act

        Action act = () => usuario.Atualizar(
            "Novo Nome",
            "");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("email");
    }

    [Fact]
    public void Desativar_DeveTornarUsuarioInativo()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        // Act

        usuario.Desativar();

        // Assert

        usuario.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Desativar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        // Act

        usuario.Desativar();

        // Assert

        usuario.DataAtualizacao
            .Should()
            .NotBeNull();

        usuario.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Desativar_UsuarioJaDesativado_DeveLancarException()
    {
        // Arrange

        var usuario = new Usuario(
            "Igor",
            "igor@email.com",
            "hash");

        usuario.Desativar();

        // Act

        Action act = () => usuario.Desativar();

        // Assert

        act.Should()
            .Throw<UsuarioJaDesativadoException>();
    }
}