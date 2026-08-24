using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Domain;

public class CarteiraTests
{
    [Fact]
    public void Construtor_DeveCriarCarteiraComDadosInformados()
    {
        // Arrange

        var nome = "Minha Carteira";
        var usuarioId = Guid.NewGuid();

        // Act

        var carteira = new Carteira(
            nome,
            usuarioId);

        // Assert

        carteira.Id.Should().NotBeEmpty();

        carteira.Nome.Should().Be(nome);

        carteira.UsuarioId.Should().Be(usuarioId);

        carteira.Ativa.Should().BeTrue();

        carteira.Saldo.Should().Be(0m);

        carteira.DataCriacao
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));

        carteira.DataAtualizacao.Should().BeNull();

        carteira.Posicoes.Should().BeEmpty();
    }

    [Fact]
    public void Construtor_UsuarioIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Carteira(
            "Minha Carteira",
            Guid.Empty);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("usuarioId");
    }

    [Fact]
    public void Construtor_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Carteira(
            "",
            Guid.NewGuid());

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_NomeSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Carteira(
            "   ",
            Guid.NewGuid());

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_NomeComEspacos_DeveRemoverEspacosDasExtremidades()
    {
        // Arrange

        var usuarioId = Guid.NewGuid();

        // Act

        var carteira = new Carteira(
            "  Minha Carteira  ",
            usuarioId);

        // Assert

        carteira.Nome.Should().Be("Minha Carteira");
    }

    [Fact]
    public void Atualizar_DeveAlterarNome()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira Antiga",
            Guid.NewGuid());

        // Act

        carteira.Atualizar("Carteira Nova");

        // Assert

        carteira.Nome.Should().Be("Carteira Nova");
    }

    [Fact]
    public void Atualizar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        // Act

        carteira.Atualizar("Nova Carteira");

        // Assert

        carteira.DataAtualizacao
            .Should()
            .NotBeNull();

        carteira.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Atualizar_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        // Act

        Action act = () => carteira.Atualizar("");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Atualizar_NomeComEspacos_DeveRemoverEspacosDasExtremidades()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        // Act

        carteira.Atualizar("  Nova Carteira  ");

        // Assert

        carteira.Nome.Should().Be("Nova Carteira");
    }

    [Fact]
    public void Desativar_DeveTornarCarteiraInativa()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        // Act

        carteira.Desativar();

        // Assert

        carteira.Ativa.Should().BeFalse();
    }

    [Fact]
    public void Desativar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        // Act

        carteira.Desativar();

        // Assert

        carteira.DataAtualizacao
            .Should()
            .NotBeNull();

        carteira.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Desativar_CarteiraJaDesativada_DeveLancarException()
    {
        // Arrange

        var carteira = new Carteira(
            "Carteira",
            Guid.NewGuid());

        carteira.Desativar();

        // Act

        Action act = () => carteira.Desativar();

        // Assert

        act.Should()
            .Throw<CarteiraJaDesativadaException>();
    }
}