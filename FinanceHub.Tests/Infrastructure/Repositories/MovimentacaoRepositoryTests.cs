using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class MovimentacaoRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public MovimentacaoRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
    }

    [Fact]
    public async Task CriarAsync_DeveAdicionarMovimentacao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var posicao =
            await CriarPosicaoAsync(context);

        var movimentacao =
            Movimentacao.CriarCompra(
                posicao.Id,
                10m,
                35m);

        var repository =
            new MovimentacaoRepository(context);

        await repository.CriarAsync(movimentacao);

        context.Entry(movimentacao)
            .State
            .Should()
            .Be(EntityState.Added);
    }

    [Fact]
    public async Task BuscarPorPosicaoAsync_DeveRetornarMovimentacoesDaPosicao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var posicao =
            await CriarPosicaoAsync(context);

        var movimentacao1 =
            Movimentacao.CriarCompra(
                posicao.Id,
                10m,
                35m);

        var movimentacao2 =
            Movimentacao.CriarVenda(
                posicao.Id,
                5m,
                40m);

        await context.Movimentacoes.AddRangeAsync(
            movimentacao1,
            movimentacao2);

        await context.SaveChangesAsync();

        var repository =
            new MovimentacaoRepository(context);

        var result =
            await repository.BuscarPorPosicaoAsync(
                posicao.Id);

        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == movimentacao1.Id);
        result.Should().Contain(x => x.Id == movimentacao2.Id);
    }

    [Fact]
    public async Task BuscarPorPosicaoAsync_PosicaoSemMovimentacoes_DeveRetornarVazio()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var posicao =
            await CriarPosicaoAsync(context);

        var repository =
            new MovimentacaoRepository(context);

        var result =
            await repository.BuscarPorPosicaoAsync(
                posicao.Id);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task BuscarPorPosicaoAsync_DeveOrdenarPorDataDecrescente()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var posicao =
            await CriarPosicaoAsync(context);

        var movimentacao1 =
            Movimentacao.CriarCompra(
                posicao.Id,
                10m,
                35m);

        await context.Movimentacoes.AddAsync(
            movimentacao1);

        await context.SaveChangesAsync();

        await Task.Delay(20);

        var movimentacao2 =
            Movimentacao.CriarCompra(
                posicao.Id,
                5m,
                40m);

        await context.Movimentacoes.AddAsync(
            movimentacao2);

        await context.SaveChangesAsync();

        var repository =
            new MovimentacaoRepository(context);

        var result =
            (await repository.BuscarPorPosicaoAsync(
                posicao.Id))
            .ToList();

        result[0].Id.Should().Be(movimentacao2.Id);
        result[1].Id.Should().Be(movimentacao1.Id);
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_DevePersistirMovimentacao()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var posicao =
            await CriarPosicaoAsync(context);

        var movimentacao =
            Movimentacao.CriarCompra(
                posicao.Id,
                10m,
                35m);

        var repository =
            new MovimentacaoRepository(context);

        await repository.CriarAsync(movimentacao);

        await repository.SalvarAlteracoesAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Movimentacoes
                .FirstOrDefaultAsync(x =>
                    x.Id == movimentacao.Id);

        resultado.Should().NotBeNull();
        resultado!.PosicaoId.Should().Be(posicao.Id);
    }

    private static async Task<Usuario> CriarUsuarioAsync(
    AppDbContext context)
    {
        var usuario = new Usuario(
            "Usuario Teste",
            $"teste{Guid.NewGuid():N}@email.com",
            "hash");

        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();

        return usuario;
    }

    private static async Task<Posicao> CriarPosicaoAsync(
    AppDbContext context)
    {
        var usuario =
            await CriarUsuarioAsync(context);

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

        return posicao;
    }
}