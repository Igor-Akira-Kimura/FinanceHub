using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class AtivoRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public AtivoRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarAtivo()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var bolsa = CriarBolsa();
        var ativo = CriarAtivo(bolsa);

        var repository = new AtivoRepository(context);

        // Act

        await context.Bolsas.AddAsync(bolsa);
        await repository.CriarAsync(ativo);

        // Assert

        context.Entry(ativo)
            .State
            .Should()
            .Be(
                Microsoft.EntityFrameworkCore.EntityState.Added);
    }

    [Fact]
    public async Task BuscarPorIdAsync_AtivoExistente_DeveRetornarAtivo()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (bolsa, ativo) =
            await CriarEAdicionarAtivoAsync(context);

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarPorIdAsync(ativo.Id);

        // Assert

        result.Should().NotBeNull();

        result!.Id
            .Should()
            .Be(ativo.Id);

        result.Nome
            .Should()
            .Be(ativo.Nome);

        result.Ticker
            .Should()
            .Be(ativo.Ticker);

        result.Bolsa
            .Should()
            .NotBeNull();

        result.Bolsa.Id
            .Should()
            .Be(bolsa.Id);
    }

    [Fact]
    public async Task BuscarPorIdAsync_AtivoInexistente_DeveRetornarNull()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var repository = new AtivoRepository(context);

        var id = Guid.NewGuid();

        // Act

        var result =
            await repository.BuscarPorIdAsync(id);

        // Assert

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdAsync_AtivoDesativado_DeveRetornarNull()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, ativo) =
            await CriarEAdicionarAtivoAsync(context);

        ativo.Desativar();

        await context.SaveChangesAsync();

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarPorIdAsync(ativo.Id);

        // Assert

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdLeituraAsync_AtivoExistente_DeveRetornarAtivoSemTracking()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, ativo) =
            await CriarEAdicionarAtivoAsync(context);

        context.ChangeTracker.Clear();

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarPorIdLeituraAsync(ativo.Id);

        // Assert

        result.Should().NotBeNull();

        result!.Id
            .Should()
            .Be(ativo.Id);

        context.Entry(result)
            .State
            .Should()
            .Be(
                Microsoft.EntityFrameworkCore.EntityState.Detached);
    }

    [Fact]
    public async Task BuscarPorIdLeituraAsync_AtivoInexistente_DeveRetornarNull()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var repository = new AtivoRepository(context);

        var id = Guid.NewGuid();

        // Act

        var result =
            await repository.BuscarPorIdLeituraAsync(id);

        // Assert

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorTickerAsync_AtivoExistente_DeveRetornarAtivo()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, ativo) =
            await CriarEAdicionarAtivoAsync(context);

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarPorTickerAsync(ativo.Ticker);

        // Assert

        result.Should().NotBeNull();

        result!.Id
            .Should()
            .Be(ativo.Id);

        result.Ticker
            .Should()
            .Be(ativo.Ticker);
    }

    [Fact]
    public async Task BuscarPorTickerAsync_TickerInexistente_DeveRetornarNull()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarPorTickerAsync("NAOEXISTE");

        // Assert

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorTickerAsync_AtivoDesativado_DeveRetornarNull()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, ativo) =
            await CriarEAdicionarAtivoAsync(context);

        ativo.Desativar();

        await context.SaveChangesAsync();

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarPorTickerAsync(ativo.Ticker);

        // Assert

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarTodosAsync_DeveRetornarAtivosAtivos()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, ativo1) =
            await CriarEAdicionarAtivoAsync(context);

        var (_, ativo2) =
            await CriarEAdicionarAtivoAsync(context);

        ativo2.Desativar();

        await context.SaveChangesAsync();

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarTodosAsync();

        // Assert

        result.Should().Contain(a => a.Id == ativo1.Id);

        result.Should().NotContain(a => a.Id == ativo2.Id);
    }

    [Fact]
    public async Task BuscarTodosAsync_DeveRetornarBolsaDosAtivos()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var (bolsa, ativo) =
            await CriarEAdicionarAtivoAsync(context);

        var repository = new AtivoRepository(context);

        // Act

        var result =
            await repository.BuscarTodosAsync();

        // Assert

        var encontrado =
            result.Single(a => a.Id == ativo.Id);

        encontrado.Bolsa
            .Should()
            .NotBeNull();

        encontrado.Bolsa.Id
            .Should()
            .Be(bolsa.Id);
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_DevePersistirAlteracoes()
    {
        // Arrange

        await using var context =
            new AppDbContext(_fixture.Options);

        var bolsa = CriarBolsa();
        var ativo = CriarAtivo(bolsa);

        await context.Bolsas.AddAsync(bolsa);

        var repository = new AtivoRepository(context);

        await repository.CriarAsync(ativo);

        // Act

        await repository.SalvarAlteracoesAsync();

        // Assert

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var ativoPersistido =
            await outroContext.Ativos
                .FirstOrDefaultAsync(a => a.Id == ativo.Id);

        ativoPersistido.Should().NotBeNull();

        ativoPersistido!.Ticker
            .Should()
            .Be(ativo.Ticker);
    }

    private static Bolsa CriarBolsa()
    {
        return new Bolsa(
            "B3",
            "Brasil",
            "BRL");
    }

    private static Ativo CriarAtivo(Bolsa bolsa)
    {
        var ticker =
            $"TST{Guid.NewGuid():N}"
                .ToUpperInvariant()
                .Substring(0, 10);

        return new Ativo(
            "ATIVO TESTE",
            ticker,
            TipoAtivo.Acao,
            bolsa.Id,
            35m);
    }

    private static async Task<(Bolsa Bolsa, Ativo Ativo)>
        CriarEAdicionarAtivoAsync(AppDbContext context)
    {
        var bolsa = CriarBolsa();
        var ativo = CriarAtivo(bolsa);

        await context.Bolsas.AddAsync(bolsa);
        await context.Ativos.AddAsync(ativo);
        await context.SaveChangesAsync();

        return (bolsa, ativo);
    }
}