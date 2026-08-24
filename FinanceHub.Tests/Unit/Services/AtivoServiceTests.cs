using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Services;
using FinanceHub.Application.Validators;
using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace FinanceHub.Tests.Unit.Services;

public class AtivoServiceTests
{
    private readonly Mock<IAtivoRepository> _ativoRepository = new();
    private readonly Mock<IBolsaRepository> _bolsaRepository = new();
    private readonly CriarAtivoRequestValidator _criarAtivoRequestValidator = new();

    private AtivoService CriarService()
    {
        return new AtivoService(
            _ativoRepository.Object,
            _bolsaRepository.Object,
            _criarAtivoRequestValidator);
    }

    private static Ativo CriarAtivoComBolsa(
        string nome,
        string ticker,
        decimal preco)
    {
        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        var ativo = new Ativo(
            nome,
            ticker,
            TipoAtivo.Acao,
            bolsa.Id,
            preco);

        typeof(Ativo)
            .GetProperty(nameof(Ativo.Bolsa))!
            .SetValue(ativo, bolsa);

        return ativo;
    }

    [Fact]
    public async Task CriarAsync_TickerJaCadastrado_DeveLancarException()
    {
        // Arrange

        var bolsaId = Guid.NewGuid();

        var request = new CriarAtivoRequest
        {
            Nome = "PETROBRAS",
            Ticker = "PETR4",
            Tipo = TipoAtivo.Acao,
            BolsaId = bolsaId,
            Preco = 35m
        };

        var ativoExistente = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            bolsaId,
            35m);

        _ativoRepository
            .Setup(x => x.BuscarPorTickerAsync(request.Ticker))
            .ReturnsAsync(ativoExistente);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CriarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<TickerJaCadastradoException>();

        _bolsaRepository.Verify(
            x => x.BuscarPorIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_BolsaNaoEncontrada_DeveLancarException()
    {
        // Arrange

        var bolsaId = Guid.NewGuid();

        var request = new CriarAtivoRequest
        {
            Nome = "PETROBRAS",
            Ticker = "PETR4",
            Tipo = TipoAtivo.Acao,
            BolsaId = bolsaId,
            Preco = 35m
        };

        _ativoRepository
            .Setup(x => x.BuscarPorTickerAsync(request.Ticker))
            .ReturnsAsync((Ativo?)null);

        _bolsaRepository
            .Setup(x => x.BuscarPorIdAsync(bolsaId))
            .ReturnsAsync((Bolsa?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.CriarAsync(request);

        // Assert

        await act.Should()
            .ThrowAsync<BolsaNaoEncontradaException>();

        _ativoRepository.Verify(
            x => x.CriarAsync(It.IsAny<Ativo>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_DeveCriarAtivo()
    {
        // Arrange

        var bolsaId = Guid.NewGuid();

        var bolsa = new Bolsa(
            "B3",
            "Brasil",
            "BRL");

        var request = new CriarAtivoRequest
        {
            Nome = "PETROBRAS",
            Ticker = "PETR4",
            Tipo = TipoAtivo.Acao,
            BolsaId = bolsaId,
            Preco = 35m
        };

        _ativoRepository
            .Setup(x => x.BuscarPorTickerAsync(request.Ticker))
            .ReturnsAsync((Ativo?)null);

        _bolsaRepository
            .Setup(x => x.BuscarPorIdAsync(bolsaId))
            .ReturnsAsync(bolsa);

        var service = CriarService();

        // Act

        var result = await service.CriarAsync(request);

        // Assert

        result.Nome.Should().Be(request.Nome);
        result.Ticker.Should().Be(request.Ticker);

        _ativoRepository.Verify(
            x => x.CriarAsync(It.Is<Ativo>(a =>
                a.Nome == request.Nome &&
                a.Ticker == request.Ticker &&
                a.BolsaId == request.BolsaId &&
                a.Preco == request.Preco)),
            Times.Once);

        _ativoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_AtivoNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Ativo?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.BuscarPorIdAsync(id);

        // Assert

        await act.Should()
            .ThrowAsync<AtivoNaoEncontradoException>();
    }

    [Fact]
    public async Task BuscarPorIdAsync_AtivoExistente_DeveRetornarResponse()
    {
        // Arrange

        var ativo = CriarAtivoComBolsa(
            "PETROBRAS",
            "PETR4",
            35m);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        var service = CriarService();

        // Act

        var result = await service.BuscarPorIdAsync(ativo.Id);

        // Assert

        result.Id.Should().Be(ativo.Id);
        result.Nome.Should().Be("PETROBRAS");
        result.Ticker.Should().Be("PETR4");
        result.Tipo.Should().Be(TipoAtivo.Acao);
        result.Bolsa.Should().Be("B3");
    }

    [Fact]
    public async Task BuscarTodosAsync_DeveRetornarTodosOsAtivos()
    {
        // Arrange

        var ativo1 = CriarAtivoComBolsa(
            "PETROBRAS",
            "PETR4",
            35m);

        var ativo2 = CriarAtivoComBolsa(
            "VALE",
            "VALE3",
            60m);

        var ativos = new[]
        {
            ativo1,
            ativo2
        };

        _ativoRepository
            .Setup(x => x.BuscarTodosAsync())
            .ReturnsAsync(ativos);

        var service = CriarService();

        // Act

        var result = (await service.BuscarTodosAsync())
            .ToList();

        // Assert

        result.Should().HaveCount(2);

        result[0].Ticker.Should().Be("PETR4");
        result[0].Bolsa.Should().Be("B3");

        result[1].Ticker.Should().Be("VALE3");
        result[1].Bolsa.Should().Be("B3");
    }

    [Fact]
    public async Task AtualizarAsync_AtivoNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        var request = new AtualizarAtivoRequest
        {
            Nome = "PETROBRAS",
            Ticker = "PETR4",
            Tipo = TipoAtivo.Acao
        };

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Ativo?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.AtualizarAsync(id, request);

        // Assert

        await act.Should()
            .ThrowAsync<AtivoNaoEncontradoException>();
    }

    [Fact]
    public async Task AtualizarAsync_TickerPertenceAOutroAtivo_DeveLancarException()
    {
        // Arrange

        var bolsaId = Guid.NewGuid();

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            bolsaId,
            35m);

        var outroAtivo = new Ativo(
            "VALE",
            "VALE3",
            TipoAtivo.Acao,
            bolsaId,
            60m);

        var request = new AtualizarAtivoRequest
        {
            Nome = "PETROBRAS",
            Ticker = "VALE3",
            Tipo = TipoAtivo.Acao
        };

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _ativoRepository
            .Setup(x => x.BuscarPorTickerAsync(request.Ticker))
            .ReturnsAsync(outroAtivo);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.AtualizarAsync(ativo.Id, request);

        // Assert

        await act.Should()
            .ThrowAsync<TickerJaCadastradoException>();

        _ativoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DadosValidos_DeveAtualizar()
    {
        // Arrange

        var bolsaId = Guid.NewGuid();

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            bolsaId,
            35m);

        var request = new AtualizarAtivoRequest
        {
            Nome = "PETROBRAS PN",
            Ticker = "PETR4",
            Tipo = TipoAtivo.Acao
        };

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        _ativoRepository
            .Setup(x => x.BuscarPorTickerAsync(request.Ticker))
            .ReturnsAsync(ativo);

        var service = CriarService();

        // Act

        await service.AtualizarAsync(ativo.Id, request);

        // Assert

        ativo.Nome.Should().Be("PETROBRAS PN");

        _ativoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task DesativarAsync_AtivoNaoEncontrado_DeveLancarException()
    {
        // Arrange

        var id = Guid.NewGuid();

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(id))
            .ReturnsAsync((Ativo?)null);

        var service = CriarService();

        // Act

        Func<Task> act = () => service.DesativarAsync(id);

        // Assert

        await act.Should()
            .ThrowAsync<AtivoNaoEncontradoException>();
    }

    [Fact]
    public async Task DesativarAsync_AtivoExistente_DeveDesativar()
    {
        // Arrange

        var ativo = new Ativo(
            "PETROBRAS",
            "PETR4",
            TipoAtivo.Acao,
            Guid.NewGuid(),
            35m);

        _ativoRepository
            .Setup(x => x.BuscarPorIdAsync(ativo.Id))
            .ReturnsAsync(ativo);

        var service = CriarService();

        // Act

        await service.DesativarAsync(ativo.Id);

        // Assert

        ativo.EstaAtivo.Should().BeFalse();

        _ativoRepository.Verify(
            x => x.SalvarAlteracoesAsync(),
            Times.Once);
    }
}