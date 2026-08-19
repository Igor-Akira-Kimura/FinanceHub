using FinanceHub.Domain.Entities;
using FinanceHub.Infrastructure.Data;
using FinanceHub.Infrastructure.Repositories;
using FinanceHub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FinanceHub.Tests.Infrastructure.Repositories;

public class CarteiraRepositoryTests
    : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public CarteiraRepositoryTests(DatabaseFixture fixture)
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
    public async Task CriarAsync_DeveAdicionarCarteira()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new CarteiraRepository(context);

        var usuarioId = Guid.NewGuid();

        var carteira =
            new Carteira(
                "Carteira Teste",
                usuarioId);

        await repository.CriarAsync(carteira);

        context.Entry(carteira)
            .State
            .Should()
            .Be(
                Microsoft.EntityFrameworkCore.EntityState.Added);
    }

    [Fact]
    public async Task BuscarPorIdAsync_CarteiraExistente_DeveRetornarCarteira()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario =
            await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Carteira Teste",
                usuario.Id);

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var result =
            await repository.BuscarPorIdAsync(carteira.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(carteira.Id);
    }

    [Fact]
    public async Task BuscarPorIdAsync_CarteiraInexistente_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var repository =
            new CarteiraRepository(context);

        var result =
            await repository.BuscarPorIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdAsync_CarteiraInativa_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario =
            await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Carteira Teste",
                usuario.Id);

        carteira.Desativar();

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var result =
            await repository.BuscarPorIdAsync(carteira.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarPorIdComPosicoesAsync_DeveCarregarPosicoes()
    {
        // Este teste depende da configuração/constructor de Posicao.
        // Vamos fechar quando você me mandar Posicao.cs.
    }

    [Fact]
    public async Task BuscarPorNomeAsync_CarteiraExistente_DeveRetornarCarteira()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario =
            await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Minha Carteira",
                usuario.Id);

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var result =
            await repository.BuscarPorNomeAsync(
                usuario.Id,
                "Minha Carteira");

        result.Should().NotBeNull();
        result!.Id.Should().Be(carteira.Id);
    }

    [Fact]
    public async Task BuscarPorNomeAsync_UsuarioDiferente_DeveRetornarNull()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario =
            await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Minha Carteira",
                usuario.Id);

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var result =
            await repository.BuscarPorNomeAsync(
                Guid.NewGuid(),
                carteira.Nome);

        result.Should().BeNull();
    }

    [Fact]
    public async Task BuscarTodasAsync_DeveRetornarSomenteCarteirasAtivasDoUsuario()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = await CriarUsuarioAsync(context);

        var outroUsuario =
        await CriarUsuarioAsync(context);

        var carteira1 =
            new Carteira("Carteira 1", usuario.Id);

        var carteira2 =
            new Carteira("Carteira 2", usuario.Id);

        var outraCarteira =
            new Carteira(
                "Outra",
                outroUsuario.Id);

        carteira2.Desativar();

        await context.Carteiras.AddRangeAsync(
            carteira1,
            carteira2,
            outraCarteira);

        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var result =
            await repository.BuscarTodasAsync(usuario.Id);

        result.Should().Contain(x => x.Id == carteira1.Id);
        result.Should().NotContain(x => x.Id == carteira2.Id);
        result.Should().NotContain(x => x.Id == outraCarteira.Id);
    }

    [Fact]
    public async Task DebitarSaldoAsync_SaldoSuficiente_DeveDebitar()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario =
            await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Carteira Teste",
                usuario.Id);

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        await context.Carteiras
            .Where(x => x.Id == carteira.Id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    x => x.Saldo,
                    1000m));

        var repository =
            new CarteiraRepository(context);

        var linhasAfetadas =
            await repository.DebitarSaldoAsync(
                carteira.Id,
                300m);

        linhasAfetadas.Should().Be(1);

        var saldo =
            await context.Carteiras
                .Where(x => x.Id == carteira.Id)
                .Select(x => x.Saldo)
                .SingleAsync();

        saldo.Should().Be(700m);
    }

    [Fact]
    public async Task DebitarSaldoAsync_SaldoInsuficiente_NaoDeveDebitar()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Carteira Teste",
                usuario.Id);

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var linhasAfetadas =
            await repository.DebitarSaldoAsync(
                carteira.Id,
                300m);

        linhasAfetadas.Should().Be(0);
    }

    [Fact]
    public async Task DebitarSaldoAsync_CarteiraInativa_NaoDeveDebitar()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Carteira Teste",
                usuario.Id);

        carteira.Desativar();

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        var linhasAfetadas =
            await repository.DebitarSaldoAsync(
                carteira.Id,
                100m);

        linhasAfetadas.Should().Be(0);
    }

    [Fact]
    public async Task SalvarAlteracoesAsync_DevePersistirAlteracoes()
    {
        await using var context =
            new AppDbContext(_fixture.Options);

        var usuario = await CriarUsuarioAsync(context);

        var carteira =
            new Carteira(
                "Carteira Original",
                usuario.Id);

        await context.Carteiras.AddAsync(carteira);
        await context.SaveChangesAsync();

        var repository =
            new CarteiraRepository(context);

        carteira.Atualizar("Carteira Atualizada");

        await repository.SalvarAlteracoesAsync();

        await using var outroContext =
            new AppDbContext(_fixture.Options);

        var resultado =
            await outroContext.Carteiras
                .FindAsync(carteira.Id);

        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Carteira Atualizada");
    }
}