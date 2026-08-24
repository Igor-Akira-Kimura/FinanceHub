using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Domain;

public class MovimentacaoTests
{
    [Fact]
    public void CriarCompra_DeveCriarMovimentacaoComDadosInformados()
    {
        // Arrange

        var posicaoId = Guid.NewGuid();

        var quantidade = 10m;
        var preco = 20m;

        // Act

        var movimentacao = Movimentacao.CriarCompra(
            posicaoId,
            quantidade,
            preco);

        // Assert

        movimentacao.Id.Should().NotBeEmpty();

        movimentacao.PosicaoId.Should().Be(posicaoId);

        movimentacao.Tipo.Should().Be(TipoMovimentacao.Compra);

        movimentacao.Quantidade.Should().Be(quantidade);

        movimentacao.Preco.Should().Be(preco);

        movimentacao.DataMovimentacao
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarVenda_DeveCriarMovimentacaoComDadosInformados()
    {
        // Arrange

        var posicaoId = Guid.NewGuid();

        var quantidade = 5m;
        var preco = 30m;

        // Act

        var movimentacao = Movimentacao.CriarVenda(
            posicaoId,
            quantidade,
            preco);

        // Assert

        movimentacao.Id.Should().NotBeEmpty();

        movimentacao.PosicaoId.Should().Be(posicaoId);

        movimentacao.Tipo.Should().Be(TipoMovimentacao.Venda);

        movimentacao.Quantidade.Should().Be(quantidade);

        movimentacao.Preco.Should().Be(preco);

        movimentacao.DataMovimentacao
            .Should()
            .BeCloseTo(
                DateTime.UtcNow,
                TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CriarCompra_QuantidadeMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => Movimentacao.CriarCompra(
            Guid.NewGuid(),
            0,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("quantidade");
    }

    [Fact]
    public void CriarCompra_PrecoMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => Movimentacao.CriarCompra(
            Guid.NewGuid(),
            10,
            0);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }

    [Fact]
    public void CriarVenda_QuantidadeMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => Movimentacao.CriarVenda(
            Guid.NewGuid(),
            0,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("quantidade");
    }

    [Fact]
    public void CriarVenda_PrecoMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => Movimentacao.CriarVenda(
            Guid.NewGuid(),
            10,
            0);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }

    [Fact]
    public void CriarCompra_PosicaoIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => Movimentacao.CriarCompra(
            Guid.Empty,
            10m,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("posicaoId");
    }

    [Fact]
    public void CriarVenda_PosicaoIdVazio_DeveLancarArgumentException()
    {
        // Arrange

        Action act = () => Movimentacao.CriarVenda(
            Guid.Empty,
            10m,
            20m);

        // Assert

        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("posicaoId");
    }
}