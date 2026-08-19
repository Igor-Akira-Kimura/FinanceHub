using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class PosicaoRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PosicaoRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarPosicao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, _, posicao) =
            await CriarPosicaoAsync(context);

        var repository =
            new PosicaoRepository(context);

        context.ChangeTracker.Clear();

        await repository.CriarAsync(posicao);

        context.Entry(posicao)
            .State
            .Should()
            .Be(EntityState.Added);
    }

    [Fact]
    public async Task BuscarPorCarteiraEAtivoAsync_PosicaoExistente_DeveRetornarPosicao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var (carteira, ativo, posicao) =
            await CriarPosicaoAsync(context);

        var repository =
            new PosicaoRepository(context);

        var result =
            await repository.BuscarPorCarteiraEAtivoAsync(
                carteira.Id,
                ativo.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(posicao.Id);
        result.CarteiraId.Should().Be(carteira.Id);
        result.AtivoId.Should().Be(ativo.Id);
    }

    [Fact]
    public async Task BuscarPorCarteiraEAtivoAsync_PosicaoInexistente_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new PosicaoRepository(context);

        var result =
            await repository.BuscarPorCarteiraEAtivoAsync(
                Guid.NewGuid(),
                Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoverAsync_DeveMarcarPosicaoParaRemocao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var (_, _, posicao) =
            await CriarPosicaoAsync(context);

        var repository =
            new PosicaoRepository(context);

        await repository.RemoverAsync(posicao);

        context.Entry(posicao)
            .State
            .Should()
            .Be(EntityState.Deleted);
    }

    [Fact]
    public async Task RemoverAsync_DeveRemoverPosicaoDoBanco()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var (carteira, ativo, posicao) =
            await CriarPosicaoAsync(context);

        var repository =
            new PosicaoRepository(context);

        await repository.RemoverAsync(posicao);

        await repository.SalvarAlteracoesAsync();

        var result =
            await context.Posicoes
                .FirstOrDefaultAsync(x =>
                    x.Id == posicao.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_DevePersistirAlteracoes()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var (carteira, ativo, posicao) =
            await CriarPosicaoAsync(context);

        var repository =
            new PosicaoRepository(context);

        posicao.Comprar(2, 40m);

        await repository.SalvarAlteracoesAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Posicoes
                .FirstOrDefaultAsync(x =>
                    x.Id == posicao.Id);

        resultado.Should().NotBeNull();
        resultado!.Quantidade.Should().Be(12m);
    }

    private static async Task<(Carteira Carteira, Ativo Ativo, Posicao Posicao)>
        CriarPosicaoAsync(AppDbContext context)
    {
        var usuario = new Usuario(
            "Usuario Teste",
            $"teste{Guid.NewGuid():N}@email.com",
            "hash");

        await context.Usuarios.AddAsync(usuario);

        var carteira =
            new Carteira(
                $"Carteira-{Guid.NewGuid():N}",
                usuario.Id);

        var bolsa =
            new Bolsa(
                $"B3-{Guid.NewGuid():N}",
                "Brasil",
                "BRL");

        var ativo =
            new Ativo(
                "ATIVO TESTE",
                $"TST{Guid.NewGuid():N}"
                    .Substring(0, 10)
                    .ToUpper(),
                TipoAtivo.Acao,
                bolsa.Id,
                35m);

        var posicao =
            new Posicao(
                carteira.Id,
                ativo.Id,
                10m,
                35m);

        await context.Bolsas.AddAsync(bolsa);
        await context.Ativos.AddAsync(ativo);
        await context.Carteiras.AddAsync(carteira);
        await context.Posicoes.AddAsync(posicao);

        await context.SaveChangesAsync();

        return (carteira, ativo, posicao);
    }
}