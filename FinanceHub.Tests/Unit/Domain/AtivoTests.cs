using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Domain;

public class AtivoTests
{
    [Fact]
    public void Construtor_DeveCriarAtivoComDadosInformados()
    {
        // Arrange

        var nome = "PETROBRAS";
        var ticker = "PETR4";
        var tipo = TipoAtivo.Acao;
        var bolsaId = Guid.NewGuid();
        var preco = 35m;

        // Act

        var ativo = new Ativo(
            nome,
            ticker,
            tipo,
            bolsaId,
            preco);

        // Assert

        ativo.Id.Should().NotBeEmpty();

        ativo.Nome.Should().Be(nome);

        ativo.Ticker.Should().Be(ticker);

        ativo.Tipo.Should().Be(tipo);

        ativo.BolsaId.Should().Be(bolsaId);

        ativo.Preco.Should().Be(preco);

        ativo.EstaAtivo.Should().BeTrue();

        ativo.DataCriacao
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));

        ativo.DataAtualizacao.Should().BeNull();

        ativo.Posicoes.Should().BeEmpty();
    }

    [Fact]
    public void Construtor_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_NomeSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "   ",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_TickerVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "PETROBRAS",
            "",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("ticker");
    }

    [Fact]
    public void Construtor_TickerSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "PETROBRAS",
            "   ",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("ticker");
    }

    [Fact]
    public void Construtor_PrecoZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            0m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }

    [Fact]
    public void Construtor_PrecoNegativo_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            -1m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }

    [Fact]
    public void AtualizarPreco_DeveAlterarPreco()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        ativo.AtualizarPreco(40m);

        // Assert

        ativo.Preco.Should().Be(40m);
    }

    [Fact]
    public void AtualizarPreco_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        ativo.AtualizarPreco(40m);

        // Assert

        ativo.DataAtualizacao
            .Should()
            .NotBeNull();

        ativo.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AtualizarPreco_PrecoZero_DeveLancarArgumentException()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        Action act = () => ativo.AtualizarPreco(0m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }

    [Fact]
    public void Atualizar_DeveAlterarDados()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        ativo.Atualizar(
            "VALE",
            "VALE3",
            TipoAtivo.Acao);

        // Assert

        ativo.Nome.Should().Be("VALE");

        ativo.Ticker.Should().Be("VALE3");

        ativo.Tipo.Should().Be(TipoAtivo.Acao);
    }

    [Fact]
    public void Atualizar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        ativo.Atualizar(
            "VALE",
            "VALE3",
            TipoAtivo.Acao);

        // Assert

        ativo.DataAtualizacao
            .Should()
            .NotBeNull();

        ativo.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Atualizar_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        Action act = () => ativo.Atualizar(
            "",
            "VALE3",
            TipoAtivo.Acao);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Atualizar_TickerVazio_DeveLancarArgumentException()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        Action act = () => ativo.Atualizar(
            "VALE",
            "",
            TipoAtivo.Acao);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("ticker");
    }

    [Fact]
    public void Desativar_DeveTornarAtivoInativo()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        ativo.Desativar();

        // Assert

        ativo.EstaAtivo.Should().BeFalse();
    }

    [Fact]
    public void Desativar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        // Act

        ativo.Desativar();

        // Assert

        ativo.DataAtualizacao
            .Should()
            .NotBeNull();

        ativo.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Desativar_AtivoJaDesativado_DeveLancarException()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        ativo.Desativar();

        // Act

        Action act = () => ativo.Desativar();

        // Assert

        act.Should()
            .Throw<AtivoJaDesativadoException>();
    }

    [Fact]
    public void Construtor_BolsaIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.Empty,
            35m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("bolsaId");
    }
}