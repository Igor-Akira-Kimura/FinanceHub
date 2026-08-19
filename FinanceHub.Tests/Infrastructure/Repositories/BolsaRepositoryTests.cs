using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class BolsaRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public BolsaRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task BuscarPorIdAsync_BolsaExistente_DeveRetornarBolsa()
    {
        await using var context = new AppDbContext(_fixture.Options);

        var bolsa = CriarBolsa();

        await context.Bolsas.AddAsync(bolsa);
        await context.SaveChangesAsync();

        var repository = new BolsaRepository(context);

        var result = await repository.BuscarPorIdAsync(bolsa.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(bolsa.Id);
        result.Nome.Should().Be(bolsa.Nome);
    }

    [Fact]
    public async Task BuscarPorIdAsync_BolsaInexistente_DeveRetornarNull()
    {
        await using var context = new AppDbContext(_fixture.Options);

        var repository = new BolsaRepository(context);

        var result =
            await repository.BuscarPorIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdAsync_BolsaInativa_DeveRetornarNull()
    {
        await using var context = new AppDbContext(_fixture.Options);

        var bolsa = CriarBolsa();

        bolsa.Desativar();

        await context.Bolsas.AddAsync(bolsa);
        await context.SaveChangesAsync();

        var repository = new BolsaRepository(context);

        var result =
            await repository.BuscarPorIdAsync(bolsa.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarTodasAsync_DeveRetornarSomenteBolsasAtivas()
    {
        await using var context = new AppDbContext(_fixture.Options);

        var bolsa1 = CriarBolsa("B3");
        var bolsa2 = CriarBolsa("NASDAQ");

        bolsa2.Desativar();

        await context.Bolsas.AddRangeAsync(bolsa1, bolsa2);
        await context.SaveChangesAsync();

        var repository = new BolsaRepository(context);

        var result =
            await repository.BuscarTodasAsync();

        result.Should().Contain(x => x.Id == bolsa1.Id);
        result.Should().NotContain(x => x.Id == bolsa2.Id);
    }

    [Fact]
    public async Task BuscarTodasAsync_DeveOrdenarPorNome()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var bolsa1 = CriarBolsa("ZZZ");
        var bolsa2 = CriarBolsa("AAA");

        await context.Bolsas.AddRangeAsync(
            bolsa1,
            bolsa2);

        await context.SaveChangesAsync();

        var repository =
            new BolsaRepository(context);

        var result =
            (await repository.BuscarTodasAsync())
            .ToList();

        var nomes =
            result.Select(x => x.Nome).ToList();

        nomes.Should().BeInAscendingOrder();
    }

    private static Bolsa CriarBolsa(
        string nome = "B3")
    {
        return new Bolsa(
            nome,
            "Brasil",
            "BRL");
    }
}