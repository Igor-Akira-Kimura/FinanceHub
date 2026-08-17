using FinanceHub.Domain.Entities;
using FluentAssertions;

namespace FinanceHub.Tests.Domain;

public class CompraTests
{
    [Fact]
    public void Construtor_DeveCriarCompraComDadosInformados()
    {
        // Arrange

        var carteiraId = Guid.NewGuid();
        var ativoId = Guid.NewGuid();

        var quantidade = 10;
        var preco = 20m;

        // Act

        var compra = new Compra(
            carteiraId,
            ativoId,
            quantidade,
            preco);

        // Assert

        compra.Id.Should().NotBeEmpty();

        compra.CarteiraId.Should().Be(carteiraId);

        compra.AtivoId.Should().Be(ativoId);

        compra.Quantidade.Should().Be(quantidade);

        compra.Preco.Should().Be(preco);
    }

    [Fact]
    public void Construtor_CarteiraIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Compra(
            Guid.Empty,
            Guid.NewGuid(),
            10,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("carteiraId");
    }

    [Fact]
    public void Construtor_AtivoIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Compra(
            Guid.NewGuid(),
            Guid.Empty,
            10,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("ativoId");
    }

    [Fact]
    public void Construtor_QuantidadeMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Compra(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("quantidade");
    }

    [Fact]
    public void Construtor_QuantidadeNegativa_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Compra(
            Guid.NewGuid(),
            Guid.NewGuid(),
            -1,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("quantidade");
    }

    [Fact]
    public void Construtor_PrecoMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => new Compra(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
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

        Action act = () => new Compra(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            -10m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }
}