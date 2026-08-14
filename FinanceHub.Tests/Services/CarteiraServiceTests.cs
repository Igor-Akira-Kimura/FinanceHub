using FinanceHub.Application.Requests.Carteiras;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Exceptions;
using FinanceHub.Application.Requests;
using FinanceHub.Tests.Builders;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Moq;

namespace FinanceHub.Tests.Services;

public class CarteiraServiceTests
{
    [Fact]
    public async Task CriarAsync_UsuarioValido_DeveCriarCarteira()
    {
        // Arrange

        var fixture = new CarteiraServiceFixture();

        var usuario = fixture.ConfigurarUsuarioValido();

        fixture.ConfigurarCarteiraNaoExistente(usuario.Id);

        var request = new CriarCarteiraRequest
        {
            Nome = "Carteira Principal"
        };

        // Act

        var id = await fixture.Service.CriarAsync(request);

        // Assert

        id.Should().NotBeEmpty();

        fixture.CarteiraRepository.Verify(
            x => x.CriarAsync(It.IsAny<Carteira>()),
            Times.Once);

        fixture.CarteiraRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_UsuarioNaoEncontrado_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        var usuario = new UsuarioBuilder().Build();

        fixture.ConfigurarUsuarioLogado(usuario);

        fixture.UsuarioRepository
            .Setup(x => x.BuscarPorIdAsync(usuario.Id))
            .ReturnsAsync((Usuario?)null);

        var request = new CriarCarteiraRequest
        {
            Nome = "Carteira Principal"
        };

        Func<Task> act = () => fixture.Service.CriarAsync(request);

        await act.Should()
            .ThrowAsync<UsuarioNaoEncontradoException>();
    }

    [Fact]
    public async Task CriarAsync_UsuarioInativo_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        fixture.ConfigurarUsuarioInativo();

        var request = new CriarCarteiraRequest
        {
            Nome = "Carteira Principal"
        };

        Func<Task> act = () => fixture.Service.CriarAsync(request);

        await act.Should()
            .ThrowAsync<UsuarioInativoException>();
    }

    [Fact]
    public async Task CriarAsync_CarteiraJaCadastrada_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        var usuario = fixture.ConfigurarUsuarioValido();

        fixture.ConfigurarCarteiraExistente(usuario.Id);

        var request = new CriarCarteiraRequest
        {
            Nome = "Carteira Principal"
        };

        Func<Task> act = () => fixture.Service.CriarAsync(request);

        await act.Should()
            .ThrowAsync<CarteiraJaCadastradaException>();
    }

    [Fact]
    public async Task ComprarAtivoAsync_PosicaoNaoExiste_DeveCriarPosicao()
    {
        // Arrange

        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        var ativo = new AtivoBuilder().Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.AtivoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync((Posicao?)null);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 10,
            Preco = 20
        };

        // Act

        await fixture.Service.ComprarAtivoAsync(request);

        // Assert

        fixture.PosicaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Posicao>()),
            Times.Once);

        fixture.MovimentacaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Movimentacao>()),
            Times.Once);

        fixture.MovimentacaoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task ComprarAtivoAsync_PosicaoExiste_DeveComprar()
    {
        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        var ativo = new AtivoBuilder().Build();

        var posicao = new PosicaoBuilder()
            .ComCarteira(carteira.Id)
            .ComAtivo(ativo.Id)
            .Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.AtivoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        var quantidadeAntes = posicao.Quantidade;

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 5,
            Preco = 30
        };

        await fixture.Service.ComprarAtivoAsync(request);

        posicao.Quantidade
            .Should()
            .BeGreaterThan(quantidadeAntes);

        fixture.PosicaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Posicao>()),
            Times.Never);

        fixture.MovimentacaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Movimentacao>()),
            Times.Once);
    }

    [Fact]
    public async Task ComprarAtivoAsync_CarteiraNaoEncontrada_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Carteira?)null);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = Guid.NewGuid(),
            AtivoId = Guid.NewGuid(),
            Quantidade = 10,
            Preco = 20
        };

        Func<Task> act =
            () => fixture.Service.ComprarAtivoAsync(request);

        await act.Should()
            .ThrowAsync<CarteiraNaoEncontradaException>();
    }

    [Fact]
    public async Task ComprarAtivoAsync_AtivoNaoEncontrado_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.AtivoRepository
            .Setup(x => x.BuscarPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Ativo?)null);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = Guid.NewGuid(),
            Quantidade = 10,
            Preco = 20
        };

        Func<Task> act =
            () => fixture.Service.ComprarAtivoAsync(request);

        await act.Should()
            .ThrowAsync<AtivoNaoEncontradoException>();
    }

    [Fact]
    public async Task ComprarAtivoAsync_DeveSalvarMovimentacao()
    {
        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        var ativo = new AtivoBuilder().Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.AtivoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync((Posicao?)null);

        var request = new ComprarAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 10,
            Preco = 20
        };

        await fixture.Service.ComprarAtivoAsync(request);

        fixture.MovimentacaoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task VenderAtivoAsync_PosicaoExiste_DeveVenderAtivo()
    {
        // Arrange

        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        var ativo = new AtivoBuilder().Build();

        var posicao = new PosicaoBuilder()
            .ComCarteira(carteira.Id)
            .ComAtivo(ativo.Id)
            .ComQuantidade(100)
            .Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 30,
            Preco = 25
        };

        // Act

        await fixture.Service.VenderAtivoAsync(request);

        // Assert

        posicao.Quantidade.Should().Be(70);

        fixture.MovimentacaoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Movimentacao>()),
            Times.Once);

        fixture.MovimentacaoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task VenderAtivoAsync_CarteiraNaoEncontrada_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Carteira?)null);

        var request = new VenderAtivoRequest
        {
            CarteiraId = Guid.NewGuid(),
            AtivoId = Guid.NewGuid(),
            Quantidade = 10,
            Preco = 20
        };

        Func<Task> act =
            () => fixture.Service.VenderAtivoAsync(request);

        await act.Should()
            .ThrowAsync<CarteiraNaoEncontradaException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_PosicaoNaoEncontrada_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                It.IsAny<Guid>()))
            .ReturnsAsync((Posicao?)null);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = Guid.NewGuid(),
            Quantidade = 10,
            Preco = 20
        };

        Func<Task> act =
            () => fixture.Service.VenderAtivoAsync(request);

        await act.Should()
            .ThrowAsync<PosicaoNaoEncontradaException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_QuantidadeInsuficiente_DeveLancarException()
    {
        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        var ativo = new AtivoBuilder().Build();

        var posicao = new PosicaoBuilder()
            .ComCarteira(carteira.Id)
            .ComAtivo(ativo.Id)
            .ComQuantidade(10)
            .Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 50,
            Preco = 20
        };

        Func<Task> act =
            () => fixture.Service.VenderAtivoAsync(request);

        await act.Should()
            .ThrowAsync<QuantidadeInsuficienteException>();
    }

    [Fact]
    public async Task VenderAtivoAsync_DeveSalvarMovimentacao()
    {
        var fixture = new CarteiraServiceFixture();

        var carteira = new CarteiraBuilder().Build();

        var ativo = new AtivoBuilder().Build();

        var posicao = new PosicaoBuilder()
            .ComCarteira(carteira.Id)
            .ComAtivo(ativo.Id)
            .Build();

        fixture.CarteiraRepository
            .Setup(x => x.BuscarPorIdAsync(carteira.Id))
            .ReturnsAsync(carteira);

        fixture.PosicaoRepository
            .Setup(x => x.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id))
            .ReturnsAsync(posicao);

        var request = new VenderAtivoRequest
        {
            CarteiraId = carteira.Id,
            AtivoId = ativo.Id,
            Quantidade = 5,
            Preco = 20
        };

        await fixture.Service.VenderAtivoAsync(request);

        fixture.MovimentacaoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }
}