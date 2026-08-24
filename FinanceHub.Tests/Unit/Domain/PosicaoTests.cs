using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;

namespace FinanceHub.Tests.Unit.Domain;

public class PosicaoTests
{
    [Fact]
    public void Construtor_DeveCriarPosicaoComDadosInformados()
    {
        // Arrange

        var carteiraId = Guid.NewGuid();
        var ativoId = Guid.NewGuid();

        var quantidade = 10m;
        var precoMedio = 20m;

        // Act

        var posicao = new Posicao(
            carteiraId,
            ativoId,
            quantidade,
            precoMedio);

        // Assert

        posicao.Id.Should().NotBeEmpty();

        posicao.CarteiraId.Should().Be(carteiraId);

        posicao.AtivoId.Should().Be(ativoId);

        posicao.Quantidade.Should().Be(quantidade);

        posicao.PrecoMedio.Should().Be(precoMedio);

        posicao.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        posicao.DataAtualizacao.Should().BeNull();
    }

    [Fact]
    public void Comprar_DeveAtualizarQuantidade()
    {
        // Arrange

        var posicao = new Posicao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            20);

        // Act

        posicao.Comprar(5, 30);

        // Assert

        posicao.Quantidade.Should().Be(15);

        posicao.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Comprar_DeveCalcularPrecoMedio()
    {
        // Arrange

        var posicao = new Posicao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            20);

        // Act

        posicao.Comprar(5, 30);

        // Assert

        posicao.PrecoMedio
            .Should()
            .BeApproximately(23.3333M, 0.0001M);
    }

    [Fact]
    public void Comprar_DeveCriarMovimentacaoDeCompra()
    {
        // Arrange
        var carteiraId = Guid.NewGuid();
        var ativoId = Guid.NewGuid();

        var posicao = new Posicao(
            carteiraId,
            ativoId,
            10,
            20);

        var quantidade = 5m;
        var preco = 30m;

        // Act
        var movimentacao = posicao.Comprar(
            quantidade,
            preco);

        // Assert
        movimentacao.Should().NotBeNull();

        movimentacao.PosicaoId.Should().Be(posicao.Id);

        movimentacao.Quantidade.Should().Be(quantidade);

        movimentacao.Preco.Should().Be(preco);

        movimentacao.Tipo.Should().Be(TipoMovimentacao.Compra);

        movimentacao.DataMovimentacao.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Vender_DeveRetornarMovimentacaoVenda()
    {
        // Arrange
        var posicao = new Posicao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            20);

        // Act
        var movimentacao = posicao.Vender(4, 40);

        // Assert
        movimentacao.Tipo.Should().Be(TipoMovimentacao.Venda);

        movimentacao.Quantidade.Should().Be(4);

        movimentacao.Preco.Should().Be(40);

        movimentacao.PosicaoId.Should().Be(posicao.Id);
    }

    [Fact]
    public void Vender_QuantidadeMaiorQueSaldo_DeveLancarQuantidadeInsuficienteException()
    {
        // Arrange
        var posicao = new Posicao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            20);

        // Act
        Action act = () => posicao.Vender(15, 30);

        // Assert
        act.Should()
            .Throw<QuantidadeInsuficienteException>();
    }

    [Fact]
    public void Construtor_QuantidadeMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange
        Action act = () => new Posicao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            20);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("quantidade");
    }

    [Fact]
    public void Construtor_PrecoMenorOuIgualAZero_DeveLancarArgumentException()
    {
        // Arrange
        Action act = () => new Posicao(
            Guid.NewGuid(),
            Guid.NewGuid(),
            10,
            0);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithParameterName("preco");
    }
}