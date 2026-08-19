using FinanceHub.Domain.Entities;
using FinanceHub.Domain.Enums;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class CompraRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public CompraRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetDatabase();
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

    [Fact]
    public async Task CriarAsync_DeveAdicionarCompra()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

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
                "PETROBRAS",
                $"PET{Guid.NewGuid():N}"
                    .Substring(0, 10)
                    .ToUpper(),
                TipoAtivo.Acao,
                bolsa.Id,
                35m);

        await context.Bolsas.AddAsync(bolsa);
        await context.Ativos.AddAsync(ativo);
        await context.Carteiras.AddAsync(carteira);

        await context.SaveChangesAsync();

        var compra =
            new Compra(
                carteira.Id,
                ativo.Id,
                10m,
                35m);

        var repository =
            new CompraRepository(context);

        await repository.CriarAsync(compra);

        context.Entry(compra)
            .State
            .Should()
            .Be(EntityState.Added);
    }

    [Fact]
    public async Task CriarAsync_DevePersistirCompraQuandoSalvarAlteracoes()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

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
                "PETROBRAS",
                $"PET{Guid.NewGuid():N}"
                    .Substring(0, 10)
                    .ToUpper(),
                TipoAtivo.Acao,
                bolsa.Id,
                35m);

        await context.Bolsas.AddAsync(bolsa);
        await context.Ativos.AddAsync(ativo);
        await context.Carteiras.AddAsync(carteira);

        await context.SaveChangesAsync();

        var compra =
            new Compra(
                carteira.Id,
                ativo.Id,
                10m,
                35m);

        var repository =
            new CompraRepository(context);

        await repository.CriarAsync(compra);

        await context.SaveChangesAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Compras
                .FirstOrDefaultAsync(x =>
                    x.Id == compra.Id);

        resultado.Should().NotBeNull();
        resultado!.CarteiraId.Should().Be(carteira.Id);
        resultado.AtivoId.Should().Be(ativo.Id);
        resultado.Quantidade.Should().Be(10m);
        resultado.Preco.Should().Be(35m);
    }
}