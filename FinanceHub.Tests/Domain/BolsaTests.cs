using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;

namespace FinanceHub.Tests.Domain;

public class BolsaTests
{
    [Fact]
    public void Construtor_DeveCriarBolsaComDadosInformados()
    {
        // Arrange

        var nome = "B3";
        var pais = "Brasil";
        var moeda = "BRL";

        // Act

        var bolsa = new Bolsa(
            nome,
            pais,
            moeda);

        // Assert

        bolsa.Id.Should().NotBeEmpty();

        bolsa.Nome.Should().Be(nome);

        bolsa.Pais.Should().Be(pais);

        bolsa.Moeda.Should().Be(moeda);

        bolsa.Ativa.Should().BeTrue();

        bolsa.DataCriacao
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));

        bolsa.DataAtualizacao.Should().BeNull();

        bolsa.Ativos.Should().BeEmpty();
    }

    [Fact]
    public void Construtor_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Bolsa(
            "",
            "Brasil",
            "BRL");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_NomeSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Bolsa(
            "   ",
            "Brasil",
            "BRL");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Construtor_PaisVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Bolsa(
            "B3",
            "",
            "BRL");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("pais");
    }

    [Fact]
    public void Construtor_PaisSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Bolsa(
            "B3",
            "   ",
            "BRL");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("pais");
    }

    [Fact]
    public void Construtor_MoedaVazia_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Bolsa(
            "B3",
            "Brasil",
            "");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("moeda");
    }

    [Fact]
    public void Construtor_MoedaSomenteEspacos_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Bolsa(
            "B3",
            "Brasil",
            "   ");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("moeda");
    }

    [Fact]
    public void Atualizar_DeveAlterarDados()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        bolsa.Atualizar(
            "NYSE",
            "Estados Unidos",
            "USD");

        // Assert

        bolsa.Nome.Should().Be("NYSE");

        bolsa.Pais.Should().Be("Estados Unidos");

        bolsa.Moeda.Should().Be("USD");
    }

    [Fact]
    public void Atualizar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        bolsa.Atualizar(
            "NYSE",
            "Estados Unidos",
            "USD");

        // Assert

        bolsa.DataAtualizacao
            .Should()
            .NotBeNull();

        bolsa.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Atualizar_NomeVazio_DeveLancarArgumentException()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        Action act = () => bolsa.Atualizar(
            "",
            "Brasil",
            "BRL");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("nome");
    }

    [Fact]
    public void Atualizar_PaisVazio_DeveLancarArgumentException()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        Action act = () => bolsa.Atualizar(
            "B3",
            "",
            "BRL");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("pais");
    }

    [Fact]
    public void Atualizar_MoedaVazia_DeveLancarArgumentException()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        Action act = () => bolsa.Atualizar(
            "B3",
            "Brasil",
            "");

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("moeda");
    }

    [Fact]
    public void Desativar_DeveTornarBolsaInativa()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        bolsa.Desativar();

        // Assert

        bolsa.Ativa.Should().BeFalse();
    }

    [Fact]
    public void Desativar_DeveAtualizarDataAtualizacao()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        // Act

        bolsa.Desativar();

        // Assert

        bolsa.DataAtualizacao
            .Should()
            .NotBeNull();

        bolsa.DataAtualizacao!.Value
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Desativar_BolsaJaDesativada_DeveLancarException()
    {
        // Arrange

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        bolsa.Desativar();

        // Act

        Action act = () => bolsa.Desativar();

        // Assert

        act.Should()
            .Throw<BolsaJaDesativadaException>();
    }
}