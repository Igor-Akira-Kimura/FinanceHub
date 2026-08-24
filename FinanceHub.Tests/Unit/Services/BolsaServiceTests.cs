using FinanceHub.Application.Interfaces.Repositories;
using FinanceHub.Application.Services;
using FinanceHub.Domain.Entities;
using FluentAssertions;
using Moq;

namespace FinanceHub.Tests.Unit.Services;

public class BolsaServiceTests
{
    private readonly Mock<IBolsaRepository> _bolsaRepository = new();

    private BolsaService CriarService()
    {
        return new BolsaService(
            _bolsaRepository.Object);
    }

    [Fact]
    public async Task BuscarTodasAsync_DeveRetornarTodasAsBolsas()
    {
        // Arrange

        var bolsas = new[]
        {
            new Bolsa(
                "B3",
                "Brasil",
                "BRL"),

            new Bolsa(
                "NYSE",
                "Estados Unidos",
                "USD")
        };

        _bolsaRepository
            .Setup(x => x.BuscarTodasAsync())
            .ReturnsAsync(bolsas);

        var service = CriarService();

        // Act

        var result = (await service.BuscarTodasAsync()).ToList();

        // Assert

        result.Should().HaveCount(2);

        result.Select(x => x.Nome)
            .Should()
            .Contain(new[] { "B3", "NYSE" });
    }

    [Fact]
    public async Task BuscarTodasAsync_NenhumaBolsa_DeveRetornarListaVazia()
    {
        // Arrange

        _bolsaRepository
            .Setup(x => x.BuscarTodasAsync())
            .ReturnsAsync([]);

        var service = CriarService();

        // Act

        var result = await service.BuscarTodasAsync();

        // Assert

        result.Should().BeEmpty();
    }
}